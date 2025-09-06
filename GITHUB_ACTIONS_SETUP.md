# 🚀 GitHub Actions Setup Guide

## Quick Setup

### 1. **Add GitHub Secrets**

Go to your repository → Settings → Secrets and variables → Actions

Add these secrets:

#### `SLACK_BOT_TOKEN`
```
xoxb-your-actual-slack-bot-token-here
```

#### `SLACK_CHANNELS` (JSON format)
```json
[
  {
    "Id": "C1234567890",
    "Name": "general"
  },
  {
    "Id": "C0987654321",
    "Name": "random"
  }
]
```

#### `ARCHIVE_PASSWORD` 
```
YourSecurePassword123!
```
**Choose a strong password** - you'll need this to extract your archived messages.

### 2. **Run the Workflow**

1. Go to **Actions** tab in your repository
2. Click **Manual Build and Release**
3. Click **Run workflow**
4. Configure options:
   - ✅ **Run Slack Channel Reader**: Check this box
   - 📅 **From Date**: `2024-01-01` (optional)
   - 📅 **To Date**: `2024-01-31` (optional)
   - 📦 **Create Release**: Uncheck (unless you want a release)
5. Click **Run workflow**

## 📥 How to Get Your Results

### Method 1: Download Password-Protected Artifacts (Recommended)

1. **Wait for workflow to complete** (green checkmark)
2. **Click on the workflow run**
3. **Note the run number** (you'll need this for the password!)
4. **Scroll down to "Artifacts"** section
5. **Download**:
   - `slack-messages-protected-[run-number]` - Password-protected ZIP with your JSONL files
   - `download-instructions-[run-number]` - Instructions with the password

### 🔑 Password Information
- **Password**: Your `ARCHIVE_PASSWORD` secret
- **Security**: Password is never visible in logs or run history
- **Remember**: Use the same password you set in repository secrets

### Method 2: View in Browser

1. **Download the artifacts** (as above)
2. **Extract the password-protected ZIP** using your `ARCHIVE_PASSWORD` secret
3. **Open `viewer.html`** from your local repository
4. **Drag and drop** any `.jsonl` file from the extracted archive

## 📊 What You'll Get

### Archive Files Structure
```
slack-messages-protected.zip (password-protected)
├── general/
│   ├── 2024-01.jsonl
│   └── 2024-02.jsonl
└── random/
    └── 2024-01.jsonl
```

### Summary Report
The `download-instructions-[run-number].md` contains:
- 🔑 **Password**: The extraction password for your archive
- 📈 **Statistics**: Total messages, files created
- 📁 **File breakdown**: Messages per file
- 🔍 **Instructions**: How to extract and view the data

## 🎯 Use Cases

### Archive Today's Messages
- Check **Run Slack Channel Reader**
- Leave dates empty
- Run workflow

### Archive Date Range
- Check **Run Slack Channel Reader**
- Set **From Date**: `2024-01-01`
- Set **To Date**: `2024-01-31`
- Run workflow

### Archive from Date to Now
- Check **Run Slack Channel Reader**
- Set **From Date**: `2024-01-01`
- Leave **To Date** empty
- Run workflow

## 🔧 Troubleshooting

### "Secrets not found" Error
- Make sure `SLACK_BOT_TOKEN` and `SLACK_CHANNELS` are added to repository secrets
- Check that the JSON format for `SLACK_CHANNELS` is valid

### "No messages found" 
- Check your date range
- Verify channel IDs are correct
- Ensure the bot has access to the channels

### "Workflow doesn't show up"
- Make sure you've pushed the `.github/workflows/ci.yml` file to your repository
- Check that you're on the main branch

## 💡 Pro Tips

1. **Artifacts expire after 30 days** - download them promptly
2. **Use specific date ranges** for large channels to avoid timeouts  
3. **The viewer.html file** makes browsing messages much easier
4. **Check the workflow logs** if something goes wrong
5. **Artifacts are private** - only repository collaborators can download them

## 🔄 Regular Archiving

For regular archiving, you can:
1. **Manually trigger** the workflow weekly/monthly
2. **Set up a scheduled workflow** (modify the `on:` trigger)
3. **Use the local scripts** for more frequent archiving

## 🛡️ Security Notes

- Slack tokens are stored securely as GitHub secrets
- Artifacts are only accessible to repository collaborators
- Tokens are never logged or exposed in workflow output
- Archives are automatically deleted after 30 days