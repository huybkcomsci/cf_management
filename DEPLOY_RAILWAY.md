# 🚀 Railway.app Deployment Guide

## Prerequisites

- GitHub account with repository
- Railway.app account (free tier with $5/month credit)
- Node.js and npm installed locally
- Supabase database connection string

## Step 1: Install Railway CLI

### 1.1 Install

```bash
# macOS
brew install railway

# Linux/Windows
npm install -g @railway/cli

# Verify
railway --version
```

### 1.2 Login

```bash
railway login

# This opens browser to authenticate
```

## Step 2: Prepare Project

### 2.1 Create railway.json

```json
{
  "name": "cafe-management",
  "build": {
    "builder": "dockerfile",
    "dockerfile": "Dockerfile"
  },
  "deploy": {
    "numReplicas": 1,
    "restartPolicy": "always",
    "startCommand": "dotnet CafeManagement.dll"
  }
}
```

### 2.2 Push to GitHub

```bash
git add railway.json
git commit -m "Add Railway configuration"
git push origin main
```

## Step 3: Initialize and Deploy

### 3.1 Initialize Railway Project

```bash
railway init

# Follow prompts:
# - Select or create new project
# - Choose: Docker
# - Confirm configuration
```

### 3.2 Set Environment Variables

```bash
# Set each variable
railway variables set ASPNETCORE_ENVIRONMENT Production
railway variables set ASPNETCORE_URLS http://+:8080
railway variables set CONNECTION_STRING "Host=db.ovlnwuvvegmcrrhwolgu.supabase.co;Port=5432;..."

# List all variables
railway variables list
```

### 3.3 Deploy

```bash
# Upload and deploy
railway up

# This may take 5-10 minutes
```

## Step 4: Verify Deployment

### 4.1 Check Deployment Status

```bash
# View live logs
railway logs

# View logs from a specific date
railway logs --since 5m

# Follow logs in real-time
railway logs -f
```

### 4.2 Get Deployment URL

```bash
# In Railway Dashboard
# Or via CLI
railway status

# Should output URLs like:
# https://cafe-management.up.railway.app
```

### 4.3 Test Health Endpoint

```bash
curl https://cafe-management.up.railway.app/health

# Expected response:
# {"status":"healthy","timestamp":"...","environment":"Production"}
```

## Step 5: Connect to PostgreSQL (Optional)

If using Railway's PostgreSQL instead of Supabase:

```bash
# Add PostgreSQL plugin
railway add

# Select PostgreSQL
# Railway auto-generates connection string
# Add as environment variable:
railway variables set CONNECTION_STRING "postgresql://..."
```

## Step 6: Dashboard Configuration

### 6.1 Access Railway Dashboard

Visit https://railway.app/dashboard

### 6.2 Configure Auto Deploy

1. Click on your project
2. Go to "Settings"
3. Enable "Deploy on Push" for your GitHub branch

### 6.3 View Metrics

1. Click on service
2. Go to "Metrics" tab
3. Monitor CPU, Memory, Network

## Troubleshooting

### Issue: Build fails with "Dockerfile not found"

**Solution**:

```bash
# Ensure Dockerfile is at repository root
ls -la Dockerfile

# Or specify path
railway build --dockerfile ./Dockerfile
```

### Issue: "Application crashed"

**Solution**:

```bash
# Check logs
railway logs -f

# Check recent errors
railway logs --since 1h | grep -i error
```

### Issue: "Cannot connect to database"

**Solution**:

```bash
# Verify connection string
railway variables get CONNECTION_STRING

# Check if Supabase is accessible
psql postgresql://postgres:PASSWORD@HOST/DATABASE

# Restart service
railway down
railway up
```

### Issue: "502 Bad Gateway"

**Solution**:

```bash
# Service might be starting, wait 1-2 minutes
railway logs -f

# If persistent, check health endpoint
while true; do
  curl -s https://your-app.up.railway.app/health | jq .
  sleep 5
done
```

## Common Commands

```bash
# View status
railway status

# View logs
railway logs [-f] [--since 5m]

# Environment variables
railway variables set KEY value
railway variables get KEY
railway variables list

# Restart service
railway restart

# Stop service
railway stop

# View metrics
railway metrics

# Open dashboard in browser
railway open

# Disconnect local project
railway disconnect
```

## Monitoring

### Real-time Logs

```bash
# Follow logs in terminal
railway logs -f

# Filter by log level
railway logs -f | grep "ERROR\|ERROR"
```

### Health Checks

```bash
# Create simple health check script
#!/bin/bash
while true; do
    HEALTH=$(curl -s https://your-app.up.railway.app/health)
    echo "[$(date)] $HEALTH"
    sleep 60
done
```

### Networking

```bash
# Check if app is accessible
curl -I https://your-app.up.railway.app

# Check SSL certificate
curl -vI https://your-app.up.railway.app 2>&1 | grep -i certificate
```

## Deployment Workflow

### Automated Deployment

```bash
# Push to GitHub → Railway auto-deploys
git add .
git commit -m "Fix: Issue #123"
git push origin main

# Check status
railway logs -f
```

### Manual Deployment

```bash
# Force redeploy
railway up --force

# Or in dashboard
# Services → cafe-management → Redeploy
```

## Cost & Limits

### Railway Pricing

- **Free tier**: $5/month credit (usually covers small app)
- **Pay-as-you-go**: After credit used
- **Included**: 512 MB RAM, unlimited egress

### Monitoring Usage

```bash
# Check usage in dashboard
railway open

# Or via CLI
railway status
```

## Environment Variables Checklist

```bash
railway variables set ASPNETCORE_ENVIRONMENT Production
railway variables set ASPNETCORE_URLS http://+:8080
railway variables set CONNECTION_STRING "Host=...;Password=..."
railway variables set MAX_UPLOAD_SIZE 52428800
railway variables set LOGGING_LEVEL Information
```

## Backup Strategy

Railway doesn't provide automatic backups, so:

```bash
# Schedule regular backups
# See: ./scripts/backup.sh

# Set up cron job
0 2 * * * /path/to/backup.sh
```

## Updates & Maintenance

### Update Application Code

```bash
# Make changes
git add .
git commit -m "v1.0.1: Bug fix"
git push origin main

# Auto-deploys if "Deploy on Push" enabled
# Or manually:
railway up
```

### Update Environment Variables

```bash
railway variables set KEY "new value"

# Restart service
railway restart
```

## Support

- Railway Docs: https://docs.railway.app
- Status: https://railway.app/status
- Discord Community: https://discord.gg/railway
- Email Support: support@railway.app

## Quick Start (TL;DR)

```bash
# 1. Install CLI
brew install railway

# 2. Login
railway login

# 3. Navigate to project
cd /Users/hus/CODING/DNHAN/CafeManagement

# 4. Initialize
railway init

# 5. Set variables
railway variables set CONNECTION_STRING "postgresql://..."
railway variables set ASPNETCORE_ENVIRONMENT Production

# 6. Deploy
railway up

# 7. Monitor
railway logs -f
```
