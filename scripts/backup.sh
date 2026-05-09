#!/bin/bash
# backup.sh - Database backup script for Supabase PostgreSQL

set -e

echo "💾 Database Backup Script (Supabase PostgreSQL)"
echo "==============================================="
echo ""

# Configuration
DB_HOST="${DB_HOST:-db.ovlnwuvvegmcrrhwolgu.supabase.co}"
DB_PORT="${DB_PORT:-5432}"
DB_USER="${DB_USER:-postgres}"
DB_NAME="${DB_NAME:-postgres}"
BACKUP_DIR="${BACKUP_DIR:-./backups}"
RETENTION_DAYS="${RETENTION_DAYS:-30}"
GZIP_COMPRESS="${GZIP_COMPRESS:-true}"

# Create backup directory
mkdir -p "$BACKUP_DIR"

# Timestamp
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="$BACKUP_DIR/backup_$TIMESTAMP"

echo "📋 Backup Configuration:"
echo "   Host: $DB_HOST"
echo "   Port: $DB_PORT"
echo "   User: $DB_USER"
echo "   Database: $DB_NAME"
echo "   Location: $BACKUP_DIR"
echo ""

# Check if password is set
if [ -z "$DB_PASSWORD" ]; then
    echo "❌ Error: DB_PASSWORD environment variable not set"
    exit 1
fi

# Step 1: Create backup
echo "⏳ Creating backup (this may take a few minutes)..."

export PGPASSWORD="$DB_PASSWORD"

if [ "$GZIP_COMPRESS" = true ]; then
    # Binary format with compression
    pg_dump \
        -h "$DB_HOST" \
        -p "$DB_PORT" \
        -U "$DB_USER" \
        -d "$DB_NAME" \
        -F c \
        -v \
        | gzip > "$BACKUP_FILE.dump.gz"
    
    BACKUP_SIZE=$(du -h "$BACKUP_FILE.dump.gz" | cut -f1)
    echo "   ✅ Backup saved: $BACKUP_FILE.dump.gz ($BACKUP_SIZE)"
else
    # SQL text format (larger but human-readable)
    pg_dump \
        -h "$DB_HOST" \
        -p "$DB_PORT" \
        -U "$DB_USER" \
        -d "$DB_NAME" \
        -F p \
        -v > "$BACKUP_FILE.sql"
    
    BACKUP_SIZE=$(du -h "$BACKUP_FILE.sql" | cut -f1)
    echo "   ✅ Backup saved: $BACKUP_FILE.sql ($BACKUP_SIZE)"
fi

unset PGPASSWORD

# Step 2: Cleanup old backups
echo ""
echo "🧹 Cleaning up old backups (retention: $RETENTION_DAYS days)..."

DELETED_COUNT=$(find "$BACKUP_DIR" -type f \( -name "backup_*.sql.gz" -o -name "backup_*.dump.gz" \) -mtime +$RETENTION_DAYS -delete 2>/dev/null | wc -l)
echo "   ✅ Deleted $DELETED_COUNT old backup(s)"

# Step 3: Show backup statistics
echo ""
echo "📊 Backup Statistics:"
TOTAL_SIZE=$(du -sh "$BACKUP_DIR" | cut -f1)
BACKUP_COUNT=$(find "$BACKUP_DIR" -type f \( -name "backup_*.sql.gz" -o -name "backup_*.dump.gz" \) | wc -l)
echo "   Total backups: $BACKUP_COUNT"
echo "   Total size: $TOTAL_SIZE"
echo ""

# Step 4: Log backup
cat >> "$BACKUP_DIR/backup.log" << EOF
[$(date)] Backup completed
File: $BACKUP_FILE$([ "$GZIP_COMPRESS" = true ] && echo ".dump.gz" || echo ".sql")
Size: $BACKUP_SIZE
Status: SUCCESS
Retention: $RETENTION_DAYS days

EOF

echo "✅ Backup completed successfully!"
echo ""
echo "💡 Tips:"
echo "   - Keep backups in secure location"
echo "   - Test restore procedure regularly"
echo "   - Upload backups to cloud storage (AWS S3, etc.)"
echo ""
