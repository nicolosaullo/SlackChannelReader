# 🤝 Contributing Guide

## 🌟 Development Workflow

This project uses a **Pull Request-based workflow** to ensure code quality and collaboration.

### 📋 Process Overview

1. **Create a feature branch**
2. **Make your changes**
3. **Create a Pull Request**
4. **Review and merge**

## 🚀 Step-by-Step Guide

### 1. Create a Feature Branch

```bash
# Always start from main branch
git checkout main
git pull origin main

# Create a new feature branch
git checkout -b feature/your-feature-name
# OR
git checkout -b fix/bug-description
# OR  
git checkout -b docs/documentation-update
```

### 2. Make Your Changes

```bash
# Make your code changes
# Edit files, add features, fix bugs, etc.

# Stage changes
git add .

# Commit with descriptive message
git commit -m "feat: add new feature description

- Detailed description of what changed
- Why the change was made
- Any important notes

🤖 Generated with [Claude Code](https://claude.ai/code)

Co-Authored-By: Claude <noreply@anthropic.com>"
```

### 3. Push and Create PR

```bash
# Push feature branch to GitHub
git push origin feature/your-feature-name

# Then go to GitHub and create a Pull Request
```

### 4. Using GitHub CLI (Optional)

```bash
# Install GitHub CLI: https://cli.github.com/
# Then you can create PRs from command line:

gh pr create --title "Add new feature" --body "Description of changes"
```

## 🎯 Branch Naming Conventions

- **Features**: `feature/slack-data-export`
- **Bug fixes**: `fix/authentication-error`
- **Documentation**: `docs/update-setup-guide`
- **Configuration**: `config/github-actions-update`
- **Security**: `security/fix-token-exposure`

## 📝 Commit Message Format

```
type: short description

Longer explanation if needed
- List of changes
- Why the change was made
- Any breaking changes

🤖 Generated with [Claude Code](https://claude.ai/code)

Co-Authored-By: Claude <noreply@anthropic.com>
```

### Commit Types:
- `feat:` New feature
- `fix:` Bug fix
- `docs:` Documentation changes
- `refactor:` Code refactoring
- `security:` Security improvements
- `config:` Configuration changes

## ✅ Pull Request Checklist

Before creating a PR, ensure:

- [ ] **Code builds successfully** (`dotnet build`)
- [ ] **No hardcoded secrets** or tokens
- [ ] **Documentation updated** if needed
- [ ] **Descriptive PR title and description**
- [ ] **Self-review completed**

## 🔍 What Happens in a PR

1. **Automated checks run** (PR Checks workflow)
   - ✅ Code builds successfully
   - ✅ Dependencies restore
   - ✅ Basic security checks
   - ✅ Required files exist

2. **Automated comment** appears on successful checks

3. **Manual review** (if working with others)

4. **Merge to main** after approval

## 🛡️ Security Guidelines

- **Never commit**:
  - Actual Slack tokens (`xoxb-...`)
  - Personal passwords or API keys
  - Real channel IDs (use examples)
  - Actual archive data

- **Always use**:
  - `appsettings.example.json` for templates
  - GitHub secrets for sensitive data
  - Placeholder values in documentation

## 🎨 Code Style

- Follow existing code patterns
- Use clear, descriptive variable names
- Add comments for complex logic
- Keep functions focused and small

## 📋 Example PR Workflow

```bash
# 1. Start from main
git checkout main
git pull origin main

# 2. Create feature branch
git checkout -b feature/improve-error-handling

# 3. Make changes
# ... edit files ...

# 4. Commit changes
git add .
git commit -m "feat: improve error handling for API failures

- Add retry logic for rate limiting
- Better error messages for users
- Log detailed error information

🤖 Generated with [Claude Code](https://claude.ai/code)

Co-Authored-By: Claude <noreply@anthropic.com>"

# 5. Push and create PR
git push origin feature/improve-error-handling
# Then go to GitHub to create the PR
```

## 🎉 Benefits of This Workflow

✅ **Code quality** - Automated checks catch issues early  
✅ **Review process** - Changes are reviewed before merging  
✅ **Clear history** - Each PR documents what changed and why  
✅ **Safe main branch** - Main branch stays stable  
✅ **Collaboration** - Easy to track and discuss changes  

---

**Happy coding! 🚀**