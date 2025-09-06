namespace SlackChannelReader.Services;

public interface IArchiveOrchestrator
{
    Task ArchiveChannelsAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task ArchiveChannelAsync(string channelId, string channelName, DateTime? fromDate = null, DateTime? toDate = null);
}