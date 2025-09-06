using System.Text.Json.Serialization;

namespace SlackChannelReader.Models;

public class SlackUser
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("profile")]
    public SlackUserProfile Profile { get; set; } = new();
}

public class SlackUserProfile
{
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("real_name")]
    public string RealName { get; set; } = string.Empty;
}