# 📦 Deployment Guide - ASP.NET Core MVC 8 + Supabase PostgreSQL

## 1. CHO:N PLATFORM TRIỂN KHAI

### Render (Khuyến nghị - Free tier có sẵn)

✅ Miễn phí, easy setup, PostgreSQL integration

- Deploy từ GitHub
- Automatic HTTPS
- Built-in environment variables

### Railway

✅ Miễn phí, simple UI, pay-as-you-go

- Simple integration
- Auto-deploy from Git
- Environment variables dễ quản lý

### Azure

✅ Enterprise, powerful, but paid

- App Service (dễ nhất)
- CI/CD with GitHub Actions
- Advanced monitoring

---

## 2. CHUẨN BỊ PUBLISH RELEASE

### Create Release Configuration

```bash
# 1. Clean và build release
dotnet clean
dotnet build -c Release

# 2. Publish to folder
dotnet publish -c Release -o ./publish

# 3. Size của published app
du -sh ./publish
```

### appsettings.Production.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    },
    "Console": {
      "IncludeScopes": false,
      "TimestampFormat": "yyyy-MM-dd HH:mm:ss"
    }
  },
  "AllowedHosts": "*",
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://+:8080"
      }
    }
  },
  "ApplicationSettings": {
    "MaxUploadSize": 52428800,
    "EnableDetailedErrors": false
  }
}
```

---

## 3. DEPLOYMENT STEPS

### Option A: Render.com (Khuyến nghị)

#### 3.1 Chuẩn bị Dockerfile

```bash
# Dockerfile đã tạo sẵn tại repo root
# Kiểm tra Dockerfile
cat Dockerfile
```

#### 3.2 Deploy trên Render

1. Đăng nhập vào [render.com](https://render.com)
2. Click **New +** → **Web Service**
3. Chọn **Deploy existing code from repository**
4. Kết nối GitHub repository
5. Chọn branch `main` hoặc `production`
6. Cấu hình:
   - **Name**: cafe-management
   - **Region**: Singapore hoặc gần nhất
   - **Branch**: main
   - **Runtime**: Docker
   - **Build Command**: (để trống)
   - **Start Command**: (để trống)

#### 3.3 Environment Variables

Thêm tại **Environment** tab:

```env
# Supabase PostgreSQL Connection
CONNECTION_STRING=Host=db.ovlnwuvvegmcrrhwolgu.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR-PASSWORD;SSL Mode=Require

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080

# App Settings
APP_TITLE=Cafe Management System
ENABLE_MIGRATIONS=true

# Logging
LOGGING_LEVEL=Information
```

### Option B: Railway.app

#### 3.1 Chuẩn bị

```bash
# Railway CLI
npm i -g @railway/cli

# Login
railway login
```

#### 3.2 Tạo railway.json

```json
{
  "build": {
    "builder": "dockerfile",
    "dockerfile": "Dockerfile"
  },
  "deploy": {
    "numReplicas": 1,
    "startCommand": "dotnet CafeManagement.dll"
  }
}
```

#### 3.3 Deploy

```bash
# Init project
railway init

# Deploy
railway up

# View logs
railway logs
```

#### 3.4 Environment Variables

```bash
railway variables set CONNECTION_STRING "Host=..."
railway variables set ASPNETCORE_ENVIRONMENT Production
```

### Option C: Microsoft Azure

#### 3.1 Chuẩn bị Azure CLI

```bash
# Install Azure CLI
# macOS: brew install azure-cli

# Login
az login

# Create Resource Group
az group create --name cafe-management --location Southeast Asia
```

#### 3.2 Create App Service Plan & App Service

```bash
# Create App Service Plan (Free tier)
az appservice plan create \
  --name cafe-plan \
  --resource-group cafe-management \
  --sku F1 --is-linux

# Create Web App
az webapp create \
  --resource-group cafe-management \
  --plan cafe-plan \
  --name cafe-management-app \
  --runtime "DOTNET|8.0"
```

#### 3.3 Connect PostgreSQL (Supabase)

```bash
az webapp config appsettings set \
  --name cafe-management-app \
  --resource-group cafe-management \
  --settings \
    CONNECTION_STRING="Host=..." \
    ASPNETCORE_ENVIRONMENT=Production
```

#### 3.4 Deploy từ Local

```bash
# Publish
dotnet publish -c Release -o ./publish

# Deploy ZIP
cd publish
zip -r ../app.zip .
cd ..

az webapp deployment source config-zip \
  --resource-group cafe-management \
  --name cafe-management-app \
  --src app.zip
```

---

## 4. ENVIRONMENT VARIABLES

### Connection String (Supabase)

```
CONNECTION_STRING=Host=db.ovlnwuvvegmcrrhwolgu.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR-PASSWORD;SSL Mode=Require;Timeout=30;CommandTimeout=30
```

### Required Variables

```bash
# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080

# Database
DB_HOST=db.ovlnwuvvegmcrrhwolgu.supabase.co
DB_PORT=5432
DB_USER=postgres
DB_PASSWORD=YOUR-PASSWORD
DB_NAME=postgres

# Security
CORS_ALLOWED_ORIGINS=https://your-domain.com
MAX_UPLOAD_SIZE=52428800

# Features
ENABLE_MIGRATIONS=true
ENABLE_LOGGING=true
```

---

## 5. DATABASE MIGRATION (PRODUCTION)

### 5.1 Create Migration Script

```bash
# Generate migration
dotnet ef migrations add InitialCreate \
  --project CafeManagement.csproj \
  --output-dir Migrations \
  --context ApplicationDbContext

# Generate SQL script
dotnet ef migrations script \
  --project CafeManagement.csproj \
  --idempotent \
  --output ./migrations/initial.sql
```

### 5.2 Apply Migration

#### Automatic (Recommended)

Thêm vào Program.cs:

```csharp
// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        dbContext.Database.Migrate();
        Console.WriteLine("✅ Migrations applied successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Migration failed: {ex.Message}");
        throw;
    }
}
```

#### Manual via SQL Execution

```sql
-- Execute in Supabase SQL editor
-- Chỉ chạy 1 lần, nên backup trước!
\i ./migrations/initial.sql
```

---

## 6. STATIC FILES & ASSET VERSIONING

### 6.1 Configure wwwroot

```csharp
// In Program.cs
var app = builder.Build();

// Enable static files with caching
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        const int durationInSeconds = 60 * 60 * 24 * 365; // 1 year
        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
            $"public, max-age={durationInSeconds}";
    }
});

// For dynamic content (no cache)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot",
            "content")),
    RequestPath = "/content",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
            "public, max-age=0, must-revalidate";
    }
});
```

### 6.2 Compression

```csharp
// In Program.cs
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
});

// ... after building app
app.UseResponseCompression();
```

---

## 7. HTTPS & SECURITY

### 7.1 HTTPS Enforcement

```csharp
// Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // HSTS header
    app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
}
```

### 7.2 Security Headers

```csharp
// Add security middleware
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Strict-Transport-Security",
        "max-age=31536000; includeSubDomains");
    context.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'");

    await next();
});
```

### 7.3 CORS Configuration

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecific", builder =>
    {
        builder.WithOrigins("https://your-domain.com")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// ... use CORS
app.UseCors("AllowSpecific");
```

---

## 8. LOGGING SETUP

### 8.1 Console Logging (Render/Railway friendly)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning",
      "System": "Warning"
    },
    "Console": {
      "IncludeScopes": true,
      "TimestampFormat": "yyyy-MM-dd HH:mm:ss.fff zzz"
    }
  }
}
```

### 8.2 Structured Logging (Optional: Serilog)

Install:

```bash
dotnet add package Serilog
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
```

Setup in Program.cs:

```csharp
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// ... rest of setup
```

### 8.3 Application Insights (Azure only)

```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

---

## 9. BACKUP & DISASTER RECOVERY

### 9.1 Database Backup (Supabase)

#### Manual Backup

```sql
-- Supabase Dashboard → Backups
-- Hoặc via psql
pg_dump \
  -h db.ovlnwuvvegmcrrhwolgu.supabase.co \
  -U postgres \
  -d postgres \
  -v \
  --no-password \
  > backup_$(date +%Y%m%d_%H%M%S).sql
```

#### Automated Backup Script

```bash
#!/bin/bash
# backup.sh

BACKUP_DIR="./backups"
DB_HOST="db.ovlnwuvvegmcrrhwolgu.supabase.co"
DB_USER="postgres"
DB_NAME="postgres"
BACKUP_FILE="$BACKUP_DIR/backup_$(date +%Y%m%d_%H%M%S).sql"

mkdir -p "$BACKUP_DIR"

PGPASSWORD=$DB_PASSWORD pg_dump \
  -h $DB_HOST \
  -U $DB_USER \
  -d $DB_NAME \
  -F c \
  -v \
  > "$BACKUP_FILE"

# Keep only last 30 days
find "$BACKUP_DIR" -type f -name "backup_*.sql" -mtime +30 -delete

echo "✅ Backup to $BACKUP_FILE"
```

Run với cron:

```bash
# Edit crontab
crontab -e

# Add (daily 2 AM):
0 2 * * * /path/to/backup.sh
```

### 9.2 Application Code Backup

```bash
# Git push regularly
git add .
git commit -m "Production: $(date)"
git push origin main

# Tag releases
git tag -a v1.0.0 -m "Release 1.0.0"
git push origin v1.0.0
```

### 9.3 Quick Restore Procedure

```bash
# Restore từ backup
psql -h $DB_HOST -U postgres -d postgres < backup_YYYYMMDD_HHMMSS.sql

# Or với pg_restore
pg_restore -h $DB_HOST -U postgres -d postgres backup_file.dump
```

---

## 10. MONITORING & MAINTENANCE

### 10.1 Health Check Endpoint

Add to Program.cs:

```csharp
// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName
}))
.WithName("HealthCheck")
.WithOpenApi()
.AllowAnonymous();
```

### 10.2 Deployment Checklist

- [ ] Supabase password strong & rotated
- [ ] Environment variables set correctly
- [ ] Database migrations applied
- [ ] SSL/HTTPS enabled
- [ ] CORS configured properly
- [ ] Logging enabled
- [ ] Health check working
- [ ] Static files serving correctly
- [ ] Admin account created
- [ ] Backup procedure tested

### 10.3 Post-Deployment Verification

```bash
# Test deployed app
curl -I https://your-app.render.com/

# Check health
curl https://your-app.render.com/health

# Check logs
# Render: Dashboard → Logs
# Railway: railway logs
# Azure: az webapp log tail
```

---

## 11. TROUBLESHOOTING

| Issue               | Solution                                 |
| ------------------- | ---------------------------------------- |
| Connection timeout  | Check Supabase IP whitelist, SSL=Require |
| 502 Bad Gateway     | Check logs, restart service              |
| Static files 404    | Verify wwwroot path, UseStaticFiles()    |
| Migrations fail     | Check migration SQL, apply manually      |
| OutOfMemory         | Scale up instance (Render paid plan)     |
| HTTPS redirect loop | Check forwarded headers middleware       |

---

## 📚 Additional Resources

- [Render Docker docs](https://render.com/docs/docker)
- [Railway docs](https://docs.railway.app/)
- [Azure App Service](https://docs.microsoft.com/en-us/azure/app-service/)
- [Entity Framework Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Supabase PostgreSQL](https://supabase.com/docs)
