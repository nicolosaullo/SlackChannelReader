# Slack Channel Reader

A .NET 8 console application that archives Slack channel messages into AI-friendly Markdown format.

## Features

- Archives messages from multiple Slack channels
- Handles message threading (nests replies under root messages)
- Exports to organized Markdown files by date
- Supports pagination for large channels
- Includes rate limiting and retry logic for Slack API
- Production-ready with dependency injection, logging, and configuration

## Setup

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

Edit `appsettings.json`:

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
  }
}
```

## Usage

### Build the Application

```bash
dotnet build
```

### Run Examples

```bash
# Archive today's messages from all configured channels
dotnet run

# Archive messages from a specific date range
dotnet run "2024-01-01" "2024-01-31"

# Archive messages from a specific date to today
dotnet run "2024-01-01" ""
```

## Output Structure

The application creates this file structure:

```
slack-archive/
├── #general/
│   ├── 2024-01-15.md
│   ├── 2024-01-16.md
│   └── ...
└── #random/
    ├── 2024-01-15.md
    └── ...
```

### Markdown Format

Each daily archive includes:

- Channel name and date header
- Archive metadata (generation time, message count)
- Messages in chronological order
- Threaded replies indented under parent messages
- User display names and timestamps
- Clean text formatting (Slack markup converted to Markdown)

Example:

```markdown
# #general - 2024-01-15

Archive generated on 2024-01-15 10:30:00 UTC
Total messages: 45

---

**John Doe** *(09:15:30)*
Good morning team! Ready for the sprint review?

  ↳ **Jane Smith** *(09:16:45)*
  Yes! I have the demo ready.

  ↳ **Bob Wilson** *(09:17:12)*
  Looking forward to it!

**Alice Johnson** *(10:20:15)*
Here's the updated project timeline: [Project Timeline](https://docs.example.com/timeline)
```

## Configuration Options

### Slack Options

- `Token`: Your Slack Bot User OAuth Token
- `Channels`: Array of channels to archive
  - `Id`: Slack channel ID (e.g., C1234567890)
  - `Name`: Human-readable channel name for file organization

### Archive Options

- `OutputPath`: Directory where archives are saved (default: "./slack-archive")

## Error Handling

The application includes:

- Automatic retry with exponential backoff for API failures
- Rate limiting handling (waits for retry-after headers)
- Comprehensive logging for troubleshooting
- Graceful handling of missing users/channels

## Adding New Channels

To archive additional channels:

1. Get the channel ID from Slack
2. Add it to the `Channels` array in `appsettings.json`
3. Run the application

## Limitations

- Only archives public channels (private channels require additional permissions)
- File attachments are not downloaded (only links are preserved)
- Slack reactions/emojis are not preserved in the current version
- Very large channels may take time to process due to API rate limits

## Development

The application follows clean architecture principles:

- **Services/SlackClient.cs**: Handles all Slack API interactions
- **Services/MarkdownWriter.cs**: Converts messages to Markdown format
- **Services/ArchiveOrchestrator.cs**: Coordinates the archiving process
- **Models/**: Data structures for Slack API responses
- **Configuration/**: Configuration classes for dependency injection