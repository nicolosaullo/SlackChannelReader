using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SlackChannelReader.Configuration;
using SlackChannelReader.Models;
using System.Text.Json;

namespace SlackChannelReader.Services;

public class JsonWriter : IJsonWriter
{
    private readonly ArchiveOptions _options;
    private readonly ILogger<JsonWriter> _logger;

    public JsonWriter(IOptions<ArchiveOptions> options, ILogger<JsonWriter> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task WriteChannelArchiveAsync(string channelId, string channelName, DateTime monthDate, List<SlackMessage> messages)
    {
        var channelDir = Path.Combine(_options.OutputPath, channelName);
        Directory.CreateDirectory(channelDir);

        var fileName = $"{monthDate:yyyy-MM}.jsonl";
        var filePath = Path.Combine(channelDir, fileName);

        // Sort messages by timestamp
        var sortedMessages = messages.OrderBy(m => m.ParsedTimestamp).ToList();

        var options = new JsonSerializerOptions
        {
            WriteIndented = false, // JSONL should not be indented
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        // Write as JSONL (one JSON object per line)
        var lines = new List<string>();
        foreach (var message in sortedMessages)
        {
            // Add channel context to each message
            var messageWithContext = new
            {
                schema_version = "1.0",
                channel_id = channelId,
                channel_name = channelName,
                ts = message.Timestamp,
                ts_iso = message.TsIso,
                thread_id = message.ThreadId,
                is_root = message.IsRoot,
                message_type = message.MessageType,
                user = message.UserId,
                user_display_name = message.UserDisplayName,
                actor_user = message.ActorUser,
                text = message.Text,
                mentions = message.Mentions,
                reply_count = message.ReplyCount,
                thread_ts = message.ThreadTimestamp,
                type = message.Type,
                subtype = message.Subtype
            };

            var json = JsonSerializer.Serialize(messageWithContext, options);
            lines.Add(json);
        }

        await File.WriteAllLinesAsync(filePath, lines);
        
        _logger.LogInformation("Archived {MessageCount} messages to {FilePath}", messages.Count, filePath);
    }
}

