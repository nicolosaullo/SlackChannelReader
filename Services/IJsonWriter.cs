using SlackChannelReader.Models;

namespace SlackChannelReader.Services;

public interface IJsonWriter
{
    Task WriteChannelArchiveAsync(string channelId, string channelName, DateTime monthDate, List<SlackMessage> messages);
}