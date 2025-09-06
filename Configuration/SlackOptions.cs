namespace SlackChannelReader.Configuration;

public class SlackOptions
{
    public string Token { get; set; } = string.Empty;
    public List<ChannelInfo> Channels { get; set; } = new();
}

public class ChannelInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}