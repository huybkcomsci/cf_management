#!/bin/bash
# publish.sh - Publish release build for production

set -e

echo "🔨 Starting release build process..."
echo ""

# Variables
PROJECT_FILE="CafeManagement.csproj"
PUBLISH_DIR="./publish"
BACKUP_DIR="./publish_backups"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

# Step 1: Backup current publish directory
if [ -d "$PUBLISH_DIR" ]; then
    echo "📦 Backing up current publish directory..."
    mkdir -p "$BACKUP_DIR"
    tar -czf "$BACKUP_DIR/publish_$TIMESTAMP.tar.gz" "$PUBLISH_DIR"
    echo "   ✅ Backup saved to $BACKUP_DIR/publish_$TIMESTAMP.tar.gz"
    rm -rf "$PUBLISH_DIR"
fi

# Step 2: Restore
echo "📥 Restoring NuGet packages..."
dotnet restore "$PROJECT_FILE"
echo "   ✅ Packages restored"

# Step 3: Clean
echo "🧹 Cleaning build artifacts..."
dotnet clean "$PROJECT_FILE" -c Release -v q
echo "   ✅ Cleaned"

# Step 4: Build
echo "🔨 Building project..."
dotnet build "$PROJECT_FILE" \
    -c Release \
    -v minimal \
    --no-restore
echo "   ✅ Build successful"

# Step 5: Publish
echo "📤 Publishing application..."
dotnet publish "$PROJECT_FILE" \
    -c Release \
    -o "$PUBLISH_DIR" \
    --no-build \
    --self-contained false
echo "   ✅ Published to $PUBLISH_DIR"

# Step 6: Show stats
echo ""
echo "📊 Build Statistics:"
PUBLISH_SIZE=$(du -sh "$PUBLISH_DIR" | cut -f1)
DLL_SIZE=$(du -sh "$PUBLISH_DIR/CafeManagement.dll" | cut -f1)
echo "   Published size: $PUBLISH_SIZE"
echo "   DLL size: $DLL_SIZE"
echo ""

# Step 7: Create deployment info
cat > "$PUBLISH_DIR/DEPLOYMENT_INFO.txt" << EOF
CafeManagement - Production Release
Build Date: $(date)
Version: 1.0.0
Environment: Production
Target Framework: .NET 8.0

Dependencies:
- ASP.NET Core 8.0.0
- Entity Framework Core 8.0.1
- PostgreSQL (Supabase)
- Identity Framework

Configuration Files:
- appsettings.json (base)
- appsettings.Production.json (production overrides)

Environment Variables Required:
- CONNECTION_STRING (see appsettings.Production.json)
- ASPNETCORE_ENVIRONMENT=Production
- ASPNETCORE_URLS=http://+:8080

Migration Status:
- Database migrations will run automatically
- Check logs if any errors occur

Static Files:
- All static files in wwwroot/ are included

Next Steps:
1. Download/copy publish folder to server
2. Set environment variables
3. Run: dotnet CafeManagement.dll
4. Verify at: http://localhost:8080/health
EOF

echo "✅ Release build completed successfully!"
echo ""
echo "📝 Next Steps:"
echo "   1. Review DEPLOYMENT_INFO.txt"
echo "   2. Test locally: cd publish && dotnet CafeManagement.dll"
echo "   3. Deploy publish folder to server"
echo ""
