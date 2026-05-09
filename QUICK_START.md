# 🚀 Quick Start Deployment Guide

Choose your platform and follow the steps below.

## Platform Selection

| Platform    | Cost             | Ease   | Recommended For        |
| ----------- | ---------------- | ------ | ---------------------- |
| **Render**  | Free tier        | ⭐⭐⭐ | First-time deployments |
| **Railway** | $5 credit/mo     | ⭐⭐⭐ | Small to medium apps   |
| **Azure**   | Free tier + paid | ⭐⭐   | Enterprise deployments |

---

## ⚡ RENDER.COM - 5 Minutes Setup (Easiest)

### Prerequisites

✅ GitHub account with repository
✅ Supabase database ready
✅ All files committed and pushed to `main` branch

### Steps

#### 1. Prepare Repository

```bash
# Make sure all deployment files exist
ls Dockerfile appsettings.Production.json docker-compose.yml
```

#### 2. Go to Render Dashboard

Visit: https://render.com/dashboard

#### 3. Deploy

- Click **New +** → **Web Service**
- Select **Deploy existing code from repository**
- Connect to GitHub
- Select your repository

#### 4. Configure

Fill in:

```
Name:               cafe-management-app
Environment:        Docker
Region:             Singapore
Branch:             main
Auto-Deploy:        Yes
```

#### 5. Add Environment Variables

Click **Advanced** → **Add Environment Variable**

```env
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
CONNECTION_STRING=Host=db.ovlnwuvvegmcrrhwolgu.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Timeout=30;CommandTimeout=30
```

#### 6. Deploy

Click **Create Web Service**

**⏳ Wait 3-5 minutes for deployment**

#### 7. Verify

```bash
# Get URL from Render dashboard and test
curl https://your-app.onrender.com/health

# Should see:
# {"status":"healthy","timestamp":"..."}
```

#### 8. Visit App

- **URL**: https://your-app.onrender.com
- **Email**: admin@cafemanagement.local
- **Password**: Admin@123

**⚠️ Change password immediately!**

---

## 🚂 RAILWAY.APP - 5 Minutes Setup (Simple)

### Prerequisites

✅ Node.js and npm installed
✅ GitHub repository ready
✅ Supabase database ready

### Steps

#### 1. Install Railway CLI

```bash
npm install -g @railway/cli
railway --version
```

#### 2. Login

```bash
railway login
# Browser opens for authentication
```

#### 3. Navigate to Project

```bash
cd /Users/hus/CODING/DNHAN/CafeManagement
```

#### 4. Initialize

```bash
railway init

# Follow prompts:
# - Create new project
# - Choose Docker
```

#### 5. Set Environment Variables

```bash
railway variables set ASPNETCORE_ENVIRONMENT Production
railway variables set ASPNETCORE_URLS http://+:8080
railway variables set CONNECTION_STRING "Host=db.ovlnwuvvegmcrrhwolgu.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require"
```

#### 6. Deploy

```bash
railway up

# Wait 5-10 minutes
```

#### 7. Verify

```bash
# View logs
railway logs -f

# Get URL
railway status
```

#### 8. Visit App

- **URL**: https://your-app.up.railway.app
- **Email**: admin@cafemanagement.local
- **Password**: Admin@123

---

## ☁️ AZURE - 10 Minutes Setup (Enterprise)

### Prerequisites

✅ Azure account (free tier available)
✅ Azure CLI installed
✅ GitHub repository and publish profile ready

### Steps

#### 1. Install Azure CLI

```bash
# macOS
brew install azure-cli

# Verify
az --version
```

#### 2. Login

```bash
az login
# Browser opens for authentication
```

#### 3. Create Resources

```bash
# Create resource group
az group create \
  --name cafe-management-rg \
  --location Southeast\ Asia

# Create app service plan (free tier)
az appservice plan create \
  --name cafe-app-plan \
  --resource-group cafe-management-rg \
  --sku F1 \
  --is-linux

# Create web app
az webapp create \
  --resource-group cafe-management-rg \
  --plan cafe-app-plan \
  --name cafe-management-app \
  --runtime "DOTNET|8.0"
```

#### 4. Set Environment Variables

```bash
az webapp config appsettings set \
  --resource-group cafe-management-rg \
  --name cafe-management-app \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    "CONNECTION_STRING=Host=db.ovlnwuvvegmcrrhwolgu.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require"
```

#### 5. Publish Application

```bash
# Build release
dotnet publish -c Release -o ./publish

# Create ZIP
cd publish && zip -r ../app.zip . && cd ..

# Deploy
az webapp deployment source config-zip \
  --resource-group cafe-management-rg \
  --name cafe-management-app \
  --src app.zip
```

#### 6. Verify

```bash
# Get app URL
az webapp show \
  --resource-group cafe-management-rg \
  --name cafe-management-app \
  --query defaultHostName -o tsv

# Test health
curl https://<YOUR_APP_URL>/health

# Stream logs
az webapp log tail \
  --resource-group cafe-management-rg \
  --name cafe-management-app
```

#### 7. Visit App

- **URL**: https://cafe-management-app.azurewebsites.net
- **Email**: admin@cafemanagement.local
- **Password**: Admin@123

---

## 📋 Post-Deployment Checklist

After deployment, verify:

```bash
# ✅ 1. Health Check
curl https://your-app-url/health

# ✅ 2. Database Connection
# Check logs for migration messages
# Should see: "✅ Migrations applied successfully"

# ✅ 3. Login Works
# Visit app URL → Login with admin credentials

# ✅ 4. Static Files
# Check if CSS/JS loads (inspect DevTools)

# ✅ 5. HTTPS Works
# URL should auto-redirect to HTTPS

# ✅ 6. Security Headers
curl -I https://your-app-url | grep -i "Strict-Transport-Security"

# ✅ 7. Database Backup
bash ./scripts/backup.sh

# ✅ 8. Change Admin Password
# Log in → Account → Change Password
```

---

## 🔧 Common Post-Deployment Tasks

### Run Database Migrations

```bash
# Already run on startup, but to verify:
dotnet ef database update --configuration Release
```

### Apply Database Backup

```bash
# Backup current database
bash ./scripts/backup.sh

# List available backups
ls -la backups/
```

### Update Application

```bash
# Make changes
git add .
git commit -m "v1.0.1: Update"
git push origin main

# Auto-deploys on Render/Railway
# Or manually on Azure
```

### View Logs

```bash
# Render
# Dashboard → Logs tab

# Railway
railway logs -f

# Azure
az webapp log tail --resource-group cafe-management-rg --name cafe-management-app
```

---

## 🚨 Troubleshooting

| Issue                 | Solution                                        |
| --------------------- | ----------------------------------------------- |
| Build failed          | Check logs, ensure Dockerfile exists            |
| 502 Bad Gateway       | Restart service, check health endpoint          |
| Can't connect to DB   | Verify CONNECTION_STRING, Supabase IP whitelist |
| Static files 404      | Ensure wwwroot tracked in git                   |
| Password reset needed | See account management guide                    |

---

## 🔐 Security Reminders

Before going live:

```bash
# ✅ Review SECURITY_CHECKLIST.md
cat SECURITY_CHECKLIST.md

# ✅ Change default admin password
# ✅ Verify HTTPS is enforced
# ✅ Check database encryption
# ✅ Set up database backups
# ✅ Configure monitoring/alerts
```

---

## 📚 Next Steps

1. **Read Full Details**
   - [DEPLOYMENT.md](./DEPLOYMENT.md) - Comprehensive guide
   - [DEPLOY_RENDER.md](./DEPLOY_RENDER.md) - Render specifics
   - [DEPLOY_RAILWAY.md](./DEPLOY_RAILWAY.md) - Railway specifics
   - [DEPLOY_AZURE.md](./DEPLOY_AZURE.md) - Azure specifics

2. **Setup Monitoring**
   - Configure alerts in your platform
   - Set up database backups
   - Monitor application logs

3. **Security Hardening**
   - [SECURITY_CHECKLIST.md](./SECURITY_CHECKLIST.md)
   - Update security headers
   - Configure CORS properly
   - Rotate secrets regularly

4. **Backup & Recovery**
   - Test backup process: `bash ./scripts/backup.sh`
   - Test restore process: `bash ./scripts/restore.sh`
   - Document recovery procedures

---

## 💡 Pro Tips

- **Render/Railway**: Free tier great for testing, upgrade when traffic increases
- **Azure**: Best for enterprise features, monitor costs
- **Database**: Always backup before migrations
- **Secrets**: Never commit `.env` files
- **Monitoring**: Set up alerts for high error rates
- **Updates**: Test locally first, deploy to staging, then production

---

## 🆘 Need Help?

- **Render**: https://render.com/docs
- **Railway**: https://docs.railway.app
- **Azure**: https://docs.microsoft.com/en-us/azure/
- **This Project**: See DEPLOYMENT.md for detailed troubleshooting

---

**Happy deploying! 🎉**
