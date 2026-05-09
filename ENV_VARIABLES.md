# Environment Variables Configuration Guide

## 📋 Required Variables (All Environments)

### Database Connection

```env
# Supabase PostgreSQL
CONNECTION_STRING=Host=db.ovlnwuvvegmcrrhwolgu.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR_STRONG_PASSWORD;SSL Mode=Require;Timeout=30;CommandTimeout=30;Pooling=true;MaxPoolSize=20

# OR individual components
DB_HOST=db.ovlnwuvvegmcrrhwolgu.supabase.co
DB_PORT=5432
DB_USER=postgres
DB_PASSWORD=YOUR_STRONG_PASSWORD
DB_NAME=postgres
```

## 🔧 ASP.NET Core Configuration

```env
# Environment indicator
ASPNETCORE_ENVIRONMENT=Production  # or Development, Staging

# Server URLs (for hosted environment)
ASPNETCORE_URLS=http://+:8080

# Disable development tools in production
ASPNETCORE_DETAILEDEXCEPTIONS=false
```

## 📊 Application Settings

```env
# Logging
LOGGING_LEVEL=Information           # Debug|Information|Warning|Error|Critical
LOGGING_CONSOLE_ENABLED=true
LOGGING_FILE_ENABLED=false
LOG_RETENTION_DAYS=30

# API Configuration
API_TITLE=Cafe Management System
API_VERSION=1.0.0
MAX_UPLOAD_SIZE=52428800           # 50 MB in bytes

# Features
ENABLE_MIGRATIONS=true              # Auto-apply DB migrations
ENABLE_DETAILED_ERRORS=false        # Show detailed errors (development only)
ENABLE_SWAGGER=false                # Disable API documentation in production
```

## 🔐 Security Configuration

```env
# Session
SESSION_TIMEOUT_MINUTES=480         # 8 hours
SESSION_ABSOLUTE_TIMEOUT_MINUTES=1440  # 24 hours

# CORS
CORS_ALLOWED_ORIGINS=https://your-domain.com,https://admin.your-domain.com
CORS_ALLOWED_METHODS=GET,POST,PUT,DELETE
CORS_ALLOW_CREDENTIALS=true

# CSP (Content Security Policy)
CSP_DEFAULT_SRC='self'
CSP_SCRIPT_SRC='self' 'unsafe-inline' cdnjs.cloudflare.com
CSP_STYLE_SRC='self' 'unsafe-inline' cdnjs.cloudflare.com

# HTTPS
FORCE_HTTPS=true
HSTS_MAX_AGE=31536000              # 1 year in seconds
```

## 📧 Email Configuration (Optional)

```env
# SMTP Settings for sending emails
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=your-email@gmail.com
SMTP_PASSWORD=your-app-password    # NOT your Gmail password!
SMTP_FROM=noreply@your-domain.com
SMTP_ENABLE_SSL=true
```

## 📊 Monitoring & Analytics (Optional)

```env
# Application Insights (Azure)
APPINSIGHTS_INSTRUMENTATIONKEY=your-instrumentation-key
APPINSIGHTS_ENABLED=true

# Serilog (if using structured logging)
SERILOG_LEVEL=Information
SERILOG_WRITE_TO_CONSOLE=true
SERILOG_WRITE_TO_FILE=false
```

## 🔑 API Keys & Secrets

```env
# GitHub Actions (if CI/CD)
GITHUB_TOKEN=ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxx

# External Services
EXTERNAL_API_KEY=your-api-key
SERVICE_API_BASE_URL=https://api.example.com
```

## 📝 Template Files for Each Environment

### Development (local)

```env
# .env.development (git-ignored)
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:5000
CONNECTION_STRING=Host=localhost;Port=5432;Database=cafemanagement;Username=postgres;Password=postgres;SSL Mode=Disable
LOGGING_LEVEL=Debug
ENABLE_DETAILED_ERRORS=true
ENABLE_SWAGGER=true
```

### Staging

```env
# .env.staging
ASPNETCORE_ENVIRONMENT=Staging
ASPNETCORE_URLS=http://+:8080
CONNECTION_STRING=Host=db-staging.supabase.co;...;SSL Mode=Require
LOGGING_LEVEL=Information
ENABLE_DETAILED_ERRORS=false
MAX_UPLOAD_SIZE=52428800
```

### Production

```env
# .env.production (never commit)
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
CONNECTION_STRING=Host=db.ovlnwuvvegmcrrhwolgu.supabase.co;...;SSL Mode=Require
LOGGING_LEVEL=Warning
ENABLE_DETAILED_ERRORS=false
ENABLE_SWAGGER=false
FORCE_HTTPS=true
HSTS_MAX_AGE=31536000
MAX_UPLOAD_SIZE=52428800
SESSION_TIMEOUT_MINUTES=480
CORS_ALLOWED_ORIGINS=https://cafe.your-domain.com
```

## 🚀 Setting Environment Variables by Platform

### Local Development

```bash
# Option 1: .env file (using DotEnv or custom middleware)
cp .env.example .env
# Edit .env with your values

# Option 2: User Secrets (safer)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=..."
dotnet user-secrets list

# Option 3: Environment file in IDE
# VS Code: .vscode/launch.json → env
# Visual Studio: Properties/launchSettings.json
```

### Render.com

```bash
# In Dashboard → Environment tab
# Add each variable as key=value pair
# Or CLI:
# (Render uses Dashboard UI for env vars)
```

### Railway.app

```bash
railway variables set KEY value
railway variables set CONNECTION_STRING "Host=..."
railway variables list
```

### Azure App Service

```bash
az webapp config appsettings set \
  --resource-group cafe-management-rg \
  --name cafe-management-app \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    CONNECTION_STRING="Host=..." \
    LOGGING_LEVEL=Information
```

### Docker Compose

```yaml
environment:
  ASPNETCORE_ENVIRONMENT: Production
  CONNECTION_STRING: "Host=postgres;Port=5432;Database=cafemanagement;..."
  ASPNETCORE_URLS: http://+:8080
```

## 🔒 Security Best Practices

### ✅ DO:

- Use strong passwords (minimum 16 characters, mixed case, numbers, symbols)
- Rotate passwords every 90 days
- Use environment variables for all secrets
- Never commit `.env` or secret files to git
- Use different passwords for each environment
- Enable IP-based access control if possible
- Store secrets in secure vaults (Azure Key Vault, 1Password, etc.)

### ❌ DON'T:

- Hardcode secrets in source code
- Share production credentials in emails/chat
- Use same password across environments
- Store unencrypted passwords in files
- Log sensitive data (passwords, API keys)
- Expose environment variables in error messages

## 📋 Validation Checklist

Before deploying to production:

```bash
# Run these checks
✅ CONNECTION_STRING is production database
✅ ASPNETCORE_ENVIRONMENT=Production
✅ ENABLE_DETAILED_ERRORS=false
✅ ENABLE_SWAGGER=false
✅ MAX_UPLOAD_SIZE properly set
✅ CORS_ALLOWED_ORIGINS configured
✅ LOGGING_LEVEL set appropriately
✅ HTTPS is enforced
✅ HSTS_MAX_AGE set
✅ SESSION_TIMEOUT_MINUTES reasonable
✅ No debug settings enabled
✅ All required variables are set
✅ Secrets are not in version control
✅ Database backups configured
```

## 🔄 Rotating Secrets

When passwords must be changed:

```bash
# 1. Generate new password
# 2. Update in secure vault (Azure Key Vault, etc.)
# 3. Update environment variable
# 4. Test connection
# 5. Verify application works
# 6. Document change (if procedures require)
# 7. Notify team members
# 8. Set reminder for next rotation (90 days)
```

## 📚 References

- [ASP.NET Core Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [Safe storage of secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Environment variables in Windows](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_environment_variables)
- [PostgreSQL Connection Strings](https://www.postgresql.org/docs/current/libpq-connect.html#LIBPQ-CONNSTRING)
