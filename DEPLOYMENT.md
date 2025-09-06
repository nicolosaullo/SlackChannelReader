# GitHub Actions Deployment Guide

This guide walks you through setting up automatic Slack channel archiving using GitHub Actions.

## Prerequisites

1. **Slack Bot Token**: You need a Slack Bot User OAuth Token
2. **GitHub Account**: Free account is sufficient
3. **Channel IDs**: Know which Slack channels you want to archive

## Step-by-Step Setup

### 1. Get Your Slack Bot Token

1. Go to [Slack API Apps](https://api.slack.com/apps)
2. Create a new app or select existing one
3. Go to **"OAuth & Permissions"**
4. Under **"Bot Token Scopes"**, add these scopes:
   - `channels:history` - Read messages from public channels
   - `users:read` - Read user profile information
5. Install the app to your workspace
6. Copy the **"Bot User OAuth Token"** (starts with `xoxb-`)

### 2. Get Channel IDs

For each channel you want to archive:
1. Open the channel in Slack (web browser)
2. Copy the channel ID from URL: `https://app.slack.com/client/TXXXXXXX/C1234567890`
   - Channel ID is the part after the last `/` (e.g., `C1234567890`)

### 3. Fork or Clone This Repository

```bash
# Option 1: Fork on GitHub (recommended)
# Click "Fork" button on GitHub

# Option 2: Clone and create new repo
git clone <this-repo>
cd SlackChannelReader
# Create new repo on GitHub and push
```

### 4. Configure Your Channels

Edit `SlackChannelReader/appsettings.json`:

```json
{
  "Slack": {
    "Token": "",
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
  }
}
```

**Important**: Keep `"Token": ""` empty - we'll use GitHub secrets for this.

### 5. Set Up GitHub Secrets

1. Go to your GitHub repository
2. Click **Settings** > **Secrets and variables** > **Actions**
3. Click **"New repository secret"**
4. Add secret:
   - **Name**: `SLACK_TOKEN`
   - **Value**: Your Slack bot token (the `xoxb-...` token)

### 6. Commit and Push

```bash
git add .
git commit -m "Configure Slack channels and GitHub Actions"
git push origin main
```

## How It Works

### Daily Archive (Automatic)
- **Schedule**: Runs daily at 9:00 AM UTC
- **What it does**: Archives today's messages from all configured channels
- **Output**: Creates/updates files like `slack-archive/#general/2025-01-15.md`

### Manual Archive
1. Go to **Actions** tab in GitHub
2. Click **"Archive Slack Channels"**
3. Click **"Run workflow"**
4. Optionally specify date range:
   - Leave blank = archive today only
   - Set start date = archive from that date to today
   - Set both dates = archive specific date range

### Backfill Archive (Large Date Ranges)
For archiving months/years of history:
1. Go to **Actions** tab
2. Click **"Backfill Slack Archive"**
3. Set start date, end date, and batch size (default 30 days)
4. This processes in batches to avoid timeout issues

## File Structure

The archive creates this structure in your repository:

```
slack-archive/
├── #general/
│   ├── 2025-01-15.md
│   ├── 2025-01-16.md
│   └── ...
├── #random/
│   ├── 2025-01-15.md
│   └── ...
└── #development/
    └── ...
```

## Customization

### Change Schedule
Edit `.github/workflows/archive-slack.yml`:
```yaml
schedule:
  - cron: '0 9 * * *'  # Daily at 9 AM UTC
  - cron: '0 21 * * *' # Also at 9 PM UTC
```

### Change Timezone
GitHub Actions runs in UTC. To run at 9 AM in your timezone:
- **EST (UTC-5)**: `cron: '0 14 * * *'` (2 PM UTC = 9 AM EST)
- **PST (UTC-8)**: `cron: '0 17 * * *'` (5 PM UTC = 9 AM PST)
- **CET (UTC+1)**: `cron: '0 8 * * *'` (8 AM UTC = 9 AM CET)

### Add More Channels
1. Get the channel ID (see step 2 above)
2. Add to `appsettings.json`:
```json
{
  "Id": "CNEWCHANNEL",
  "Name": "new-channel-name"
}
```
3. Commit and push

## Monitoring

### Check if it's working:
1. **Actions tab**: See workflow runs and logs
2. **Repository**: Check if `slack-archive/` folder appears
3. **Commits**: Look for automatic commits like "Archive Slack messages for 2025-01-15"

### Troubleshooting:
- **No commits**: Check if there were new messages that day
- **Build failed**: Check Actions logs for error details
- **Authentication error**: Verify your `SLACK_TOKEN` secret is correct
- **Rate limiting**: The app includes automatic retry logic

## Security Notes

- ✅ Slack token is stored securely in GitHub Secrets
- ✅ Archives are committed to your repository (private repo recommended)
- ✅ No sensitive data is logged in Actions
- ⚠️ If using public repository, archives will be public too

## Costs

- **GitHub Actions**: Free (2,000 minutes/month)
- **Storage**: Free for reasonable archive sizes
- **Rate limits**: Slack allows 50+ requests/minute (handled automatically)

## Next Steps

After setup:
1. Wait for first automatic run (or trigger manually)
2. Check the generated archives
3. Optionally run backfill for historical data
4. Set up notifications if desired (GitHub can email on workflow failures)