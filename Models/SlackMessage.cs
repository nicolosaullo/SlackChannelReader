using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SlackChannelReader.Models;

public class SlackMessage
{
    // Core Slack fields (keep original names for API compatibility)
    [JsonPropertyName("ts")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public string? UserId { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("thread_ts")]
    public string? ThreadTimestamp { get; set; }

    [JsonPropertyName("reply_count")]
    public int? ReplyCount { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("subtype")]
    public string? Subtype { get; set; }

    // Enhanced fields for JSON output
    public string TsIso => DateTimeOffset.FromUnixTimeSeconds(long.Parse(Timestamp.Split('.')[0])).ToString("yyyy-MM-ddTHH:mm:ssZ");
    
    public string ThreadId => ThreadTimestamp ?? Timestamp;
    
    public bool IsRoot => ThreadTimestamp == null || ThreadTimestamp == Timestamp;
    
    public string? UserDisplayName { get; set; }
    
    public string? ActorUser { get; set; }
    
    public List<string>? Mentions
    {
        get
        {
            if (string.IsNullOrEmpty(Text)) return null;
            var matches = Regex.Matches(Text, @"<@([A-Z0-9]+)>");
            if (matches.Count == 0) return null;
            return matches.Select(m => m.Groups[1].Value).ToList();
        }
    }
    
    public string MessageType
    {
        get
        {
            // Detect system messages
            if (Subtype == "channel_join") return "member_joined_channel";
            if (Subtype == "channel_leave") return "member_left_channel";
            if (!string.IsNullOrEmpty(Subtype)) return Subtype;
            return "message";
        }
    }

    // Keep for backward compatibility during transition
    [JsonIgnore]
    public DateTime ParsedTimestamp => DateTimeOffset.FromUnixTimeSeconds(long.Parse(Timestamp.Split('.')[0])).DateTime;
}