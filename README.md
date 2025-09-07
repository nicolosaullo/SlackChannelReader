# Slack Channel Reader

A .NET 8 console application that exports Slack channel messages to structured JSONL format for data analysis and AI processing.

## Features

- Archives messages from multiple Slack channels simultaneously
- Exports structured data in JSONL format (one JSON object per line)
- Handles threaded messages with proper threading context
- Groups messages by month for organized archives
- Includes comprehensive message metadata and user information
- Built-in rate limiting and retry logic with exponential backoff
- Production-ready with dependency injection, logging, and configuration
- Supports flexible date range filtering

## Setup

For github actions setup [click here](https://github.com/nicolosaullo/SlackChannelReader/blob/main/GITHUB_ACTIONS_SETUP.md)

### 1. Get Slack Bot Token

1. Go to [Slack API](https://api.slack.com/apps)
2. Create a new app or use existing one
3. Go to "OAuth & Permissions"
4. Add these scopes under "Bot Token Scopes":
   - `channels:history` - Read messages from public channels
   - `users:read` - Read user profile information
5. Install the app to your workspace
6. Copy the "Bot User OAuth Token" (starts with `xoxb-`)

### 2. Get Channel IDs

1. Open Slack in your browser
2. Navigate to the channel you want to archive
3. Copy the channel ID from the URL (e.g., `C1234567890`)

### 3. Configure the Application

Copy `appsettings.example.json` to `appsettings.json` and configure:

```json
{
  "Slack": {
    "Token": "xoxb-your-actual-bot-token-here",
    "Channels": [
      {
        "Id": "C1234567890",
        "Name": "general"
      },
      {
        "Id": "C0987654321", 
        "Name": "random"
      }
    ]
  },
  "Archive": {
    "OutputPath": "./slack-archive"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

## Usage

### Build and Run

```bash
dotnet build
```

```bash
# Archive today's messages from all configured channels
dotnet run

# Archive messages from a specific date range
dotnet run "2024-01-01" "2024-01-31"

# Archive messages from a specific date to today
dotnet run "2024-01-01"
```

## Output Structure

The application creates monthly JSONL files organized by channel:

```
slack-archive/
├── general/
│   ├── 2024-01.jsonl
│   ├── 2024-02.jsonl
│   └── ...
└── random/
    ├── 2024-01.jsonl
    └── ...
```

### JSONL Format

Each line contains a complete message object with rich metadata:

```jsonl
{"schema_version":"1.0","channel_id":"C1234567890","channel_name":"general","ts":"1705123456.789","ts_iso":"2024-01-13T10:30:56Z","thread_id":"1705123456.789","is_root":true,"message_type":"message","user":"U1234567","user_display_name":"John Doe","actor_user":null,"text":"Good morning team! Ready for the sprint review?","mentions":null,"reply_count":2,"thread_ts":null,"type":"message","subtype":null}
{"schema_version":"1.0","channel_id":"C1234567890","channel_name":"general","ts":"1705123567.123","ts_iso":"2024-01-13T10:32:47Z","thread_id":"1705123456.789","is_root":false,"message_type":"message","user":"U7654321","user_display_name":"Jane Smith","actor_user":null,"text":"Yes! I have the demo ready.","mentions":null,"reply_count":null,"thread_ts":"1705123456.789","type":"message","subtype":null}
```

### Message Schema

Each message includes:

- **schema_version**: Format version for compatibility
- **channel_id/channel_name**: Channel identification
- **ts/ts_iso**: Unix timestamp and ISO 8601 formatted time
- **thread_id**: Thread identifier (root message timestamp)
- **is_root**: Whether this is a root message or reply
- **message_type**: Categorized message type (message, member_joined_channel, etc.)
- **user/user_display_name**: User ID and resolved display name
- **text**: Message content
- **mentions**: Array of mentioned user IDs (if any)
- **reply_count**: Number of replies to this message
- **thread_ts**: Thread timestamp for replies

## Architecture

The application uses clean architecture with dependency injection:

- **Program.cs**: Application entry point and DI configuration
- **Services/SlackClient.cs**: Slack API client with retry logic and user caching
- **Services/JsonWriter.cs**: JSONL export functionality  
- **Services/ArchiveOrchestrator.cs**: Coordinates parallel channel archiving
- **Models/SlackMessage.cs**: Message data model with computed properties
- **Configuration/**: Strongly-typed configuration classes

## Configuration Options

### Slack Configuration
- **Token**: Slack Bot User OAuth Token (required)
- **Channels**: Array of channel configurations
  - **Id**: Slack channel ID (e.g., C1234567890)
  - **Name**: Human-readable name for file organization

### Archive Configuration
- **OutputPath**: Output directory path (default: "./slack-archive")

### Logging Configuration
- Configurable log levels for different components
- Console logging enabled by default

## Error Handling

- **Rate Limiting**: Respects Slack API rate limits with retry-after headers
- **Exponential Backoff**: Automatic retry with increasing delays
- **User Caching**: Reduces API calls by caching user information
- **Graceful Degradation**: Continues processing if individual messages fail
- **Comprehensive Logging**: Detailed logging for monitoring and troubleshooting

## Data Features

- **Thread Preservation**: Maintains complete thread context
- **User Resolution**: Resolves user IDs to display names
- **Message Classification**: Categorizes system messages and regular messages
- **Mention Extraction**: Identifies and extracts user mentions
- **Timestamp Normalization**: Provides both Unix and ISO timestamps

## Use Cases

- Data analysis of Slack communications
- AI/ML training data preparation  
- Compliance and archival requirements
- Migration to other platforms
- Analytics and reporting

## Limitations

- Public channels only (private channels need additional permissions)
- File attachments referenced by URL only (not downloaded)
- Emoji reactions not captured
- Large channels process sequentially due to API rate limits

