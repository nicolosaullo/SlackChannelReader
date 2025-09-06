using SlackChannelReader.Models;

namespace SlackChannelReader.Services;

public interface ISlackClient
{
    Task<List<SlackMessage>> GetChannelHistoryAsync(string channelId, DateTime? from = null, DateTime? to = null);
    Task<List<SlackMessage>> GetThreadRepliesAsync(string channelId, string threadTimestamp);
    Task<SlackUser?> GetUserInfoAsync(string userId);
}