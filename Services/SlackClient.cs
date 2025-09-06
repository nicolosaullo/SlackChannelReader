using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SlackChannelReader.Configuration;
using SlackChannelReader.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SlackChannelReader.Services;

public class SlackClient : ISlackClient
{
    private readonly HttpClient _httpClient;
    private readonly SlackOptions _options;
    private readonly ILogger<SlackClient> _logger;
    private readonly Dictionary<string, SlackUser> _userCache = new();

    public SlackClient(HttpClient httpClient, IOptions<SlackOptions> options, ILogger<SlackClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        _httpClient.BaseAddress = new Uri("https://slack.com/api/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.Token);
    }

    public async Task<List<SlackMessage>> GetChannelHistoryAsync(string channelId, DateTime? from = null, DateTime? to = null)
    {
        var messages = new List<SlackMessage>();
        string? cursor = null;

        do
        {
            var url = $"conversations.history?channel={channelId}&limit=200";
            
            if (from.HasValue)
            {
                var fromUnix = ((DateTimeOffset)from.Value).ToUnixTimeSeconds();
                url += $"&oldest={fromUnix}";
            }
            
            if (to.HasValue)
            {
                var toUnix = ((DateTimeOffset)to.Value).ToUnixTimeSeconds();
                url += $"&latest={toUnix}";
            }
            
            if (!string.IsNullOrEmpty(cursor))
            {
                url += $"&cursor={cursor}";
            }

            var response = await GetWithRetryAsync<ConversationHistoryResponse>(url);
            
            if (response == null || response.Messages == null)
                break;

            foreach (var message in response.Messages)
            {
                if (!string.IsNullOrEmpty(message.UserId))
                {
                    message.UserDisplayName = await GetUserDisplayNameAsync(message.UserId);
                }
            }

            messages.AddRange(response.Messages);
            cursor = response.ResponseMetadata?.NextCursor;

        } while (!string.IsNullOrEmpty(cursor));

        return messages.OrderBy(m => m.ParsedTimestamp).ToList();
    }

    public async Task<List<SlackMessage>> GetThreadRepliesAsync(string channelId, string threadTimestamp)
    {
        var url = $"conversations.replies?channel={channelId}&ts={threadTimestamp}";
        var response = await GetWithRetryAsync<ConversationHistoryResponse>(url);
        
        if (response?.Messages == null)
            return new List<SlackMessage>();

        foreach (var message in response.Messages)
        {
            if (!string.IsNullOrEmpty(message.UserId))
            {
                message.UserDisplayName = await GetUserDisplayNameAsync(message.UserId);
            }
        }

        return response.Messages.OrderBy(m => m.ParsedTimestamp).ToList();
    }

    public async Task<SlackUser?> GetUserInfoAsync(string userId)
    {
        if (_userCache.TryGetValue(userId, out var cachedUser))
            return cachedUser;

        var url = $"users.info?user={userId}";
        var response = await GetWithRetryAsync<UserInfoResponse>(url);
        
        if (response?.User != null)
        {
            _userCache[userId] = response.User;
            return response.User;
        }

        return null;
    }

    private async Task<string> GetUserDisplayNameAsync(string userId)
    {
        var user = await GetUserInfoAsync(userId);
        
        if (user == null)
            return $"User-{userId}";

        return !string.IsNullOrEmpty(user.Profile.DisplayName) 
            ? user.Profile.DisplayName 
            : !string.IsNullOrEmpty(user.Profile.RealName) 
                ? user.Profile.RealName 
                : user.Name;
    }

    private async Task<T?> GetWithRetryAsync<T>(string url) where T : class
    {
        const int maxRetries = 3;
        var delay = TimeSpan.FromSeconds(1);

        for (int retry = 0; retry < maxRetries; retry++)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(60);
                    _logger.LogWarning("Rate limited. Waiting {Delay} seconds before retry {Retry}/{MaxRetries}", 
                        retryAfter.TotalSeconds, retry + 1, maxRetries);
                    await Task.Delay(retryAfter);
                    continue;
                }

                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                
                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;
                
                if (!root.GetProperty("ok").GetBoolean())
                {
                    var error = root.TryGetProperty("error", out var errorProp) ? errorProp.GetString() : "Unknown error";
                    _logger.LogError("Slack API error: {Error}", error);
                    return null;
                }

                return JsonSerializer.Deserialize<T>(json);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "HTTP request failed on retry {Retry}/{MaxRetries}", retry + 1, maxRetries);
                
                if (retry == maxRetries - 1)
                    throw;
                    
                await Task.Delay(delay);
                delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2); // Exponential backoff
            }
        }

        return null;
    }
}