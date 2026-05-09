# 🚀 Render.com Deployment Guide

## Prerequisites

- GitHub account with repository
- Render.com account (free tier available)
- Supabase database connection string

## Step 1: Prepare Repository

### 1.1 Ensure Dockerfile exists

```bash
# Verify Dockerfile in repository root
git ls-files | grep Dockerfile
```

### 1.2 Configure appsettings.Production.json

The file should have placeholder for CONNECTION_STRING that will be set via env var:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```

### 1.3 Push to GitHub

```bash
git add .
git commit -m "Prepare for Render deployment"
git push origin main
```

## Step 2: Create Render Service

### 2.1 Login to Render Dashboard

Go to https://render.com/dashboard

### 2.2 Click "New +" → "Web Service"

### 2.3 Connect GitHub

- Select "Connect a GitHub repository"
- Authorize Render to access GitHub
- Select your repository

### 2.4 Configure Service

Fill in the following:

```
Name:                   cafe-management-app
Environment:            Docker
Region:                 Singapore (or nearest to you)
Branch:                 main
Auto-Deploy:            Yes (enabled)
```

### 2.5 Set Environment Variables

Click "Advanced" → "Add Environment Variable"

Add each variable:

```env
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
CONNECTION_STRING=Host=db.ovlnwuvvegmcrrhwolgu.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Timeout=30;CommandTimeout=30;Pooling=true;MaxPoolSize=20
```

⚠️ **IMPORTANT**: Replace `YOUR_PASSWORD` with your actual Supabase password

### 2.6 Deploy Settings

```
Instance Type:  Free tier (0.5 CPU, 512 MB RAM)
Max Age For Max Connections: 0
```

### 2.7 Create Web Service

Click "Create Web Service"

**Wait for deployment (3-5 minutes)**

## Step 3: Monitor Deployment

### 3.1 View Logs

In Render dashboard:

- Click on your service
- Go to "Logs" tab
- Watch for build and startup messages

### 3.2 Check Health

Once deployed:

```bash
curl https://cafe-management-app.onrender.com/health

# Should return:
# {"status":"healthy","timestamp":"2024-05-13...","environment":"Production","version":"1.0.0"}
```

### 3.3 Verify Database Connection

Look for in logs:

```
✅ Migrations applied successfully
✅ Seeding data...
```

## Step 4: Post-Deployment

### 4.1 First Login

- URL: `https://cafe-management-app.onrender.com`
- Email: `admin@cafemanagement.local`
- Password: `Admin@123`

⚠️ Change this password immediately in production!

### 4.2 Run Migrations

If migrations didn't run automatically:

```bash
# SSH into Render container (if available)
# Or restart the service in Render dashboard
```

### 4.3 Setup Custom Domain

1. Go to Settings → Custom Domain
2. Add your domain (e.g., cafe.example.com)
3. Update DNS records with provided Render DNS info

## Troubleshooting

### Issue: "Build failed"

**Solution**:

- Check Dockerfile syntax
- Verify appsettings.Production.json exists
- Check logs for specific error

### Issue: "502 Bad Gateway"

**Solution**:

- Check if app is running: `curl https://your-app.onrender.com/health`
- Restart service in Render dashboard
- Check logs for crashes

### Issue: "Cannot connect to database"

**Solution**:

- Verify `CONNECTION_STRING` environment variable is set correctly
- Check Supabase IP whitelist includes Render IPs
- Test connection locally first

### Issue: Static files returning 404

**Solution**:

- Ensure `wwwroot` folder is in `.gitignore` is NOT added (should be tracked)
- Rebuild and redeploy

## Monitoring

### View Real-time Logs

```bash
# In Render dashboard:
# Service → Logs tab → select "Build log" or "Runtime log"
```

### Check Health Status

```bash
# Daily health check
curl https://cafe-management-app.onrender.com/health
```

### Monitor CPU/Memory

In Render dashboard → Metrics tab

## Cost Considerations

| Item                    | Cost                      |
| ----------------------- | ------------------------- |
| Web Service (free tier) | Free (512MB RAM, 0.5 CPU) |
| Bandwidth               | Included in free tier     |
| Database (Supabase)     | Free tier limits apply    |
| Custom domain           | Free                      |

## Scaling to Paid Tier

When free tier becomes insufficient:

1. Click "Add Instance" in service settings
2. Select "Starter" plan ($12/month)
3. More CPU and RAM for production workload

## Backup & Recovery

Render provides automatic snapshots, but you should also:

### Backup Database

```bash
# See backup.sh script
./scripts/backup.sh
```

### Monitor for Issues

- Render sends alerts to email (if configured)
- Check dashboard daily in production

## Updates & Deployments

### Deploy New Version

```bash
# Push to GitHub
git add .
git commit -m "v1.0.1: Bug fixes"
git push origin main

# Render auto-deploys within 1-2 minutes
# View progress in Logs tab
```

## Support

- Render Docs: https://render.com/docs
- Status: https://status.render.com
- Support: https://render.com/support
