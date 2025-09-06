using System.Text.Json.Serialization;

namespace SlackChannelReader.Models;

public class SlackApiResponse<T>
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }
}

public class ConversationHistoryResponse
{
    [JsonPropertyName("messages")]
    public List<SlackMessage> Messages { get; set; } = new();

    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }

    [JsonPropertyName("response_metadata")]
    public ResponseMetadata? ResponseMetadata { get; set; }
}

public class ResponseMetadata
{
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}

public class UserInfoResponse
{
    [JsonPropertyName("user")]
    public SlackUser User { get; set; } = new();
}