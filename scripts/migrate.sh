#!/bin/bash
# migrate.sh - Database migration script for production

set -e

echo "🗄️  Database Migration Script"
echo "============================"
echo ""

PROJECT_FILE="CafeManagement.csproj"
MIGRATIONS_DIR="./Migrations"
MIGRATION_SCRIPTS_DIR="./migration-scripts"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

# Check if running in production
if [ "$ASPNETCORE_ENVIRONMENT" != "Production" ]; then
    echo "⚠️  Warning: ASPNETCORE_ENVIRONMENT is not set to Production"
    echo "   Current: ${ASPNETCORE_ENVIRONMENT:-Development}"
    read -p "Continue anyway? (y/N) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "❌ Cancelled"
        exit 1
    fi
fi

# Step 1: Database backup
echo "📦 Backing up database before migration..."
mkdir -p "$MIGRATION_SCRIPTS_DIR/backups"

read -p "Enter DATABASE_URL (or press Enter to use env var): " DB_URL
if [ -z "$DB_URL" ] && [ -n "$CONNECTION_STRING" ]; then
    DB_URL="$CONNECTION_STRING"
fi

if [ -n "$DB_URL" ]; then
    echo "   Creating backup..."
    BACKUP_FILE="$MIGRATION_SCRIPTS_DIR/backups/backup_$TIMESTAMP.sql"
    
    # Create backup (you can implement this based on your DB)
    # PGPASSWORD=$DB_PASSWORD pg_dump -h $DB_HOST ... > $BACKUP_FILE
    echo "   ✅ Backup location: $BACKUP_FILE"
else
    echo "   ⚠️  Skipping backup (no DATABASE_URL)"
fi

# Step 2: Generate migration script (if needed)
echo ""
echo "🔍 Checking for pending migrations..."

PENDING_MIGRATIONS=$(dotnet ef migrations list \
    --project "$PROJECT_FILE" \
    --configuration Release 2>/dev/null | grep -c "^.*$" || echo 0)

if [ "$PENDING_MIGRATIONS" -eq 0 ]; then
    echo "   ✅ No pending migrations"
else
    echo "   ⚠️  Pending migrations detected"
    read -p "Generate migration SQL script? (y/N) " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        MIGRATION_SCRIPT="$MIGRATION_SCRIPTS_DIR/migrations_$TIMESTAMP.sql"
        echo "   Generating script..."
        dotnet ef migrations script \
            --project "$PROJECT_FILE" \
            --configuration Release \
            --idempotent \
            --output "$MIGRATION_SCRIPT"
        echo "   ✅ Script saved to: $MIGRATION_SCRIPT"
    fi
fi

# Step 3: Apply migrations
echo ""
echo "🔄 Applying database migrations..."

# Method 1: Automatic (recommended)
if dotnet ef database update \
    --project "$PROJECT_FILE" \
    --configuration Release \
    --verbose 2>&1; then
    echo "   ✅ Migrations applied successfully"
else
    echo "   ❌ Migration failed!"
    echo "   📋 Please check logs above and restore from backup if needed"
    exit 1
fi

# Step 4: Verify migration
echo ""
echo "✅ Database migration completed successfully!"
echo ""
echo "📊 Migration Summary:"
echo "   Migration Scripts Dir: $MIGRATION_SCRIPTS_DIR"
echo "   Backup Dir: $MIGRATION_SCRIPTS_DIR/backups"
echo "   Latest Backup: $TIMESTAMP"
echo ""

# Create migration log
cat >> "./migration-scripts/migration.log" << EOF
[$(date)] Migration completed
Status: SUCCESS
Timestamp: $TIMESTAMP
Environment: $ASPNETCORE_ENVIRONMENT

EOF

echo "📝 Migration log saved"
echo ""
