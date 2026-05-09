# 🚀 Microsoft Azure Deployment Guide

## Prerequisites

- Azure account (Free tier or paid subscription)
- Azure CLI installed
- .NET 8 SDK
- Supabase database connection string

## Step 1: Install Azure CLI

### 1.1 Install

```bash
# macOS
brew install azure-cli

# Linux
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# Verify
az --version
```

### 1.2 Login

```bash
az login

# This opens browser for authentication
```

## Step 2: Create Azure Resources

### 2.1 Create Resource Group

```bash
az group create \
  --name cafe-management-rg \
  --location Southeast\ Asia

# Or use different region:
# East Asia, Southeast Asia, Australia East, etc.
```

### 2.2 Create App Service Plan (Free tier)

```bash
az appservice plan create \
  --name cafe-app-plan \
  --resource-group cafe-management-rg \
  --sku F1 \
  --is-linux

# For production, upgrade to:
# --sku B2 (Basic tier, ~$55/month)
# --sku P1V2 (Premium, for high traffic)
```

### 2.3 Create Web App

```bash
az webapp create \
  --resource-group cafe-management-rg \
  --plan cafe-app-plan \
  --name cafe-management-app \
  --runtime "DOTNET|8.0"

# Get default URL (like: https://cafe-management-app.azurewebsites.net)
```

## Step 3: Configure Application Settings

### 3.1 Set Environment Variables

```bash
az webapp config appsettings set \
  --resource-group cafe-management-rg \
  --name cafe-management-app \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    "CONNECTION_STRING=Host=db.ovlnwuvvegmcrrhwolgu.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require"
```

### 3.2 Configure Application Insights (Optional)

```bash
# Create Application Insights resource
az monitor app-insights component create \
  --app cafe-insights \
  --location Southeast\ Asia \
  --resource-group cafe-management-rg \
  --application-type web

# Get instrumentation key
APPINSIGHTS_KEY=$(az monitor app-insights component show \
  --app cafe-insights \
  --resource-group cafe-management-rg \
  --query instrumentationKey -o tsv)

# Set as environment variable
az webapp config appsettings set \
  --resource-group cafe-management-rg \
  --name cafe-management-app \
  --settings \
    APPINSIGHTS_INSTRUMENTATIONKEY=$APPINSIGHTS_KEY
```

## Step 4: Publish Application

### 4.1 Create Release Build

```bash
# Navigate to project
cd /Users/hus/CODING/DNHAN/CafeManagement

# Run publish script
bash ./scripts/publish.sh

# Or manually
dotnet publish -c Release -o ./publish
```

### 4.2 Create ZIP for Deployment

```bash
cd publish
zip -r ../cafe-app.zip . -x "*.git"
cd ..

# Verify
ls -lh cafe-app.zip
```

### 4.3 Deploy to Azure

```bash
az webapp deployment source config-zip \
  --resource-group cafe-management-rg \
  --name cafe-management-app \
  --src cafe-app.zip

# Wait for deployment (2-5 minutes)
echo "Deployment started, check status..."
```

### 4.4 Check Deployment Status

```bash
# View deployment logs
az webapp deployment slot list \
  --resource-group cafe-management-rg \
  --name cafe-management-app

# Or in Portal: Deployment Center → Logs
```

## Step 5: Verify Deployment

### 5.1 Browse Application

```bash
# Get app URL
APP_URL=$(az webapp show \
  --resource-group cafe-management-rg \
  --name cafe-management-app \
  --query defaultHostName -o tsv)

echo "App URL: https://$APP_URL"

# Test health endpoint
curl https://$APP_URL/health
```

### 5.2 Stream Logs

```bash
az webapp log tail \
  --resource-group cafe-management-rg \
  --name cafe-management-app \
  --follow

# Ctrl+C to stop
```

### 5.3 Check App Status

```bash
az webapp show \
  --resource-group cafe-management-rg \
  --name cafe-management-app \
  --query state -o tsv

# Should output: Running
```

## Step 6: Configure Custom Domain

### 6.1 Add Custom Domain

```bash
az webapp config hostname add \
  --resource-group cafe-management-rg \
  --webapp-name cafe-management-app \
  --hostname cafe.example.com
```

### 6.2 Configure SSL Certificate

```bash
# Option 1: Use Azure-managed certificate (free, auto-renews)
az webapp config ssl bind \
  --resource-group cafe-management-rg \
  --name cafe-management-app \
  --certificate-thumbprint YOUR_CERT_THUMBPRINT

# Option 2: Use Let's Encrypt (recommended)
# Azure App Service supports automatic SSL certificates
# In Portal: TLS/SSL settings → Certificate management
```

## Step 7: Set Up CI/CD

### 7.1 Enable GitHub Actions (Recommended)

```bash
# In Azure Portal:
# App Service → Deployment Center → Source: GitHub
# Select repo and branch
# Select Build provider: GitHub Actions
# Azure creates workflow file automatically
```

### 7.2 Manual GitHub Actions Workflow

Create `.github/workflows/azure-deploy.yml`:

```yaml
name: Deploy to Azure

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: "8.0"

      - name: Build
        run: dotnet build -c Release

      - name: Publish
        run: dotnet publish -c Release -o ./publish

      - name: Deploy to Azure
        uses: azure/webapps-deploy@v2
        with:
          app-name: cafe-management-app
          publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE }}
          package: ./publish
```

### 7.3 Add Publish Profile Secret

```bash
# Get publish profile
az webapp deployment list-publishing-profiles \
  --resource-group cafe-management-rg \
  --name cafe-management-app \
  --query '[0].publishUrl' -o tsv

# Add as GitHub secret:
# Settings → Secrets → New repository secret
# Name: AZURE_PUBLISH_PROFILE
# Value: (paste publish profile content)
```

## Step 8: Database Migration

### 8.1 Run Migrations via SSH

```bash
# Enable SSH for App Service (Premium tier)
# Or use Azure Cloud Shell

# SSH into app
az webapp create-remote-connection \
  --resource-group cafe-management-rg \
  --name cafe-management-app

# Run migrations
cd /home/site/wwwroot
dotnet CafeManagement.dll
```

### 8.2 Or via Application Startup

Migrations run automatically on app startup (see Program.cs)

## Troubleshooting

### Issue: "Deployment failed"

**Solution**:

```bash
# Check deployment logs
az webapp deployment log show \
  --resource-group cafe-management-rg \
  --name cafe-management-app \
  --slot production

# Or stream logs
az webapp log tail \
  --resource-group cafe-management-rg \
  --name cafe-management-app
```

### Issue: "Cannot connect to database"

**Solution**:

```bash
# Verify connection string is set
az webapp config appsettings list \
  --resource-group cafe-management-rg \
  --name cafe-management-app

# Check if value is correct
az webapp config appsettings list \
  --resource-group cafe-management-rg \
  --name cafe-management-app \
  --query "[?name=='CONNECTION_STRING']" \
  -o table
```

### Issue: "502 Bad Gateway"

**Solution**:

```bash
# Restart app
az webapp restart \
  --resource-group cafe-management-rg \
  --name cafe-management-app

# Check health
curl https://cafe-management-app.azurewebsites.net/health
```

### Issue: "Static files returning 404"

**Solution**:

```bash
# Ensure wwwroot is deployed
az webapp config show \
  --resource-group cafe-management-rg \
  --name cafe-management-app \
  --query staticSiteConfig

# Redeploy
bash ./scripts/publish.sh
# Then deploy again
```

## Monitoring & Maintenance

### 7.1 View Metrics

```bash
# CPU usage
az monitor metrics list \
  --resource /subscriptions/SUBSCRIPTION_ID/resourceGroups/cafe-management-rg/providers/Microsoft.Web/sites/cafe-management-app \
  --metric "CpuTime" \
  --start-time 2024-05-13T00:00:00Z \
  --interval PT1H

# Memory usage
az monitor metrics list \
  --resource /subscriptions/SUBSCRIPTION_ID/resourceGroups/cafe-management-rg/providers/Microsoft.Web/sites/cafe-management-app \
  --metric "MemoryPercentage"
```

### 7.2 Configure Alerts

```bash
# Create alert for high CPU
az monitor metrics alert create \
  --name CpuAlert \
  --resource-group cafe-management-rg \
  --scopes /subscriptions/SUBSCRIPTION_ID/resourceGroups/cafe-management-rg/providers/Microsoft.Web/sites/cafe-management-app \
  --condition "avg CpuTime > 80" \
  --window-size 5m \
  --evaluation-frequency 1m
```

### 7.3 Configure Auto-Scale

```bash
# Create autoscale settings (Premium tier only)
az monitor autoscale create \
  --name cafe-autoscale \
  --resource-group cafe-management-rg \
  --resource cafe-app-plan \
  --resource-type "Microsoft.Web/serverFarms" \
  --enabled true \
  --min-count 1 \
  --max-count 3 \
  --count 1
```

## Backup & Disaster Recovery

### 8.1 Automated Backup (Premium tier)

```bash
# Enable automatic backups
az webapp config backup update \
  --resource-group cafe-management-rg \
  --name cafe-management-app \
  --container-url "https://storage-account.blob.core.windows.net/backups" \
  --frequency "Daily" \
  --retention 30
```

### 8.2 Manual Backup

```bash
# Create backup
az webapp config backup create \
  --resource-group cafe-management-rg \
  --name cafe-management-app

# Download backup
az webapp config backup list \
  --resource-group cafe-management-rg \
  --name cafe-management-app
```

### 8.3 Database Backup

```bash
# Use backup.sh script
bash ./scripts/backup.sh
```

## Cost Estimation

| Component            | Tier          | Cost/Month         |
| -------------------- | ------------- | ------------------ |
| App Service          | F1 (Free)     | $0                 |
| App Service          | B1 (Basic)    | ~$0.014/hour → $10 |
| PostgreSQL           | Supabase Free | $0                 |
| Application Insights | Free tier     | $0                 |
| **Total Minimum**    |               | **$0**             |
| **Recommended**      | B2            | ~$55               |

## Common Commands

```bash
# List all resources in resource group
az resource list --resource-group cafe-management-rg

# Get app configuration
az webapp config show \
  --resource-group cafe-management-rg \
  --name cafe-management-app

# Restart app
az webapp restart \
  --resource-group cafe-management-rg \
  --name cafe-management-app

# View logs
az webapp log tail \
  --resource-group cafe-management-rg \
  --name cafe-management-app

# Update settings
az webapp up \
  --name cafe-management-app \
  --resource-group cafe-management-rg
```

## Support

- Azure Docs: https://docs.microsoft.com/en-us/azure/
- App Service: https://docs.microsoft.com/en-us/azure/app-service/
- Status: https://status.azure.com
- Support: https://azure.microsoft.com/en-us/support/
