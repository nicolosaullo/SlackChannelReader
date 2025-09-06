using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SlackChannelReader.Configuration;
using SlackChannelReader.Models;

namespace SlackChannelReader.Services;

public class ArchiveOrchestrator : IArchiveOrchestrator
{
    private readonly ISlackClient _slackClient;
    private readonly IJsonWriter _jsonWriter;
    private readonly SlackOptions _slackOptions;
    private readonly ILogger<ArchiveOrchestrator> _logger;

    public ArchiveOrchestrator(
        ISlackClient slackClient, 
        IJsonWriter jsonWriter, 
        IOptions<SlackOptions> slackOptions, 
        ILogger<ArchiveOrchestrator> logger)
    {
        _slackClient = slackClient;
        _jsonWriter = jsonWriter;
        _slackOptions = slackOptions.Value;
        _logger = logger;
    }

    public async Task ArchiveChannelsAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        _logger.LogInformation("Starting archive process for {ChannelCount} channels", _slackOptions.Channels.Count);

        var tasks = _slackOptions.Channels.Select(channel => 
            ArchiveChannelAsync(channel.Id, channel.Name, fromDate, toDate));

        await Task.WhenAll(tasks);

        _logger.LogInformation("Archive process completed for all channels");
    }

    public async Task ArchiveChannelAsync(string channelId, string channelName, DateTime? fromDate = null, DateTime? toDate = null)
    {
        _logger.LogInformation("Starting archive for channel #{ChannelName} ({ChannelId})", channelName, channelId);

        try
        {
            // Get all messages from the channel
            var messages = await _slackClient.GetChannelHistoryAsync(channelId, fromDate, toDate);
            _logger.LogInformation("Retrieved {MessageCount} messages from #{ChannelName}", messages.Count, channelName);

            if (!messages.Any())
            {
                _logger.LogInformation("No messages found for #{ChannelName}", channelName);
                return;
            }

            // Fetch thread replies for messages that have threads
            var messagesWithThreads = new List<SlackMessage>();
            
            foreach (var message in messages)
            {
                messagesWithThreads.Add(message);

                if (message.ReplyCount > 0)
                {
                    _logger.LogDebug("Fetching {ReplyCount} replies for thread in #{ChannelName}", message.ReplyCount, channelName);
                    var threadReplies = await _slackClient.GetThreadRepliesAsync(channelId, message.Timestamp);
                    
                    // Add replies (skip the first message as it's the parent)
                    var replies = threadReplies.Skip(1).ToList();
                    messagesWithThreads.AddRange(replies);
                }
            }

            // Group messages by month and write monthly archives
            var messagesByMonth = messagesWithThreads
                .GroupBy(m => new DateTime(m.ParsedTimestamp.Year, m.ParsedTimestamp.Month, 1))
                .OrderBy(g => g.Key);

            foreach (var monthGroup in messagesByMonth)
            {
                var month = monthGroup.Key;
                var monthlyMessages = monthGroup.OrderBy(m => m.ParsedTimestamp).ToList();
                
                _logger.LogInformation("Writing {MessageCount} messages for #{ChannelName} for {Month}", 
                    monthlyMessages.Count, channelName, month.ToString("yyyy-MM"));

                await _jsonWriter.WriteChannelArchiveAsync(channelId, channelName, month, monthlyMessages);
            }

            _logger.LogInformation("Successfully archived #{ChannelName}", channelName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to archive channel #{ChannelName} ({ChannelId})", channelName, channelId);
            throw;
        }
    }
}