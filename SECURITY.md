# 🔒 Security Policy

## 🛡️ Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| Latest  | ✅                |
| < 1.0   | ❌                |

## 🚨 Reporting a Vulnerability

If you discover a security vulnerability, please report it by:

1. **Email**: Create an issue in this repository (public issues are fine for most security discussions)
2. **Response Time**: We aim to respond within 48 hours
3. **Updates**: Security fixes will be released as soon as possible

## 🔐 Security Best Practices

### **For Repository Owners:**

#### **GitHub Secrets Management:**
- ✅ **Store all sensitive data in GitHub Secrets**
- ✅ **Never commit real tokens or API keys**
- ✅ **Use `appsettings.example.json` for templates**
- ❌ **Never hardcode secrets in source code**

#### **Required Secrets:**
- `SLACK_BOT_TOKEN` - Your Slack Bot User OAuth Token
- `SLACK_CHANNELS` - JSON array of channels to archive
- `ARCHIVE_PASSWORD` - Password for protecting archived data

### **For Users:**

#### **Token Security:**
- 🔑 **Generate new tokens** if you suspect compromise
- 🔄 **Rotate tokens regularly** (every 90 days recommended)
- 👥 **Use dedicated bot accounts** instead of personal tokens
- 📍 **Limit token scopes** to minimum required permissions

#### **Repository Settings:**
- 🔒 **Consider making repository private** for sensitive data
- 👀 **Review collaborator access** regularly
- 🔐 **Enable two-factor authentication**

## 🎯 GitHub Actions Security

### **Workflow Security Features:**
- ✅ **Secrets are encrypted** and never visible in logs
- ✅ **Password-protected artifacts** prevent unauthorized access
- ✅ **Fresh environment** for each run (no data persistence)
- ✅ **Minimal permissions** granted to workflows

### **Artifact Security:**
- 🔐 **Archives are password-protected** using your `ARCHIVE_PASSWORD` secret
- 🚫 **Passwords are never logged** or exposed
- 📅 **Automatic cleanup** after 90 days
- 👥 **Access limited** to repository collaborators

## ⚠️ Known Security Considerations

### **Public Repository Risks:**
- 📊 **Workflow logs are public** (but secrets are redacted)
- 📁 **Artifacts are visible** to anyone with GitHub account
- 🔍 **Repository structure is visible** to everyone

### **Mitigations:**
- 🔐 **Password protection** secures actual data content
- 🚫 **No sensitive data** in source code or logs
- ✅ **Example configurations** instead of real ones

## 🛠️ Security Checklist

Before using this tool:

- [ ] ✅ **Slack bot token** stored in GitHub secrets
- [ ] ✅ **Channel configuration** stored in GitHub secrets  
- [ ] ✅ **Archive password** set in GitHub secrets
- [ ] ✅ **No real tokens** in source code
- [ ] ✅ **Repository permissions** reviewed
- [ ] ✅ **Two-factor authentication** enabled

## 📚 Additional Resources

- [GitHub Secrets Documentation](https://docs.github.com/en/actions/security-guides/encrypted-secrets)
- [Slack API Security](https://api.slack.com/authentication/best-practices)
- [GitHub Actions Security](https://docs.github.com/en/actions/security-guides)

## 🔄 Security Updates

This document is updated regularly. Last updated: $(date +%Y-%m-%d)

---

**Report security issues responsibly. Thank you for helping keep this project secure!** 🛡️