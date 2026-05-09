#!/bin/bash
# restore.sh - Database restore script from backup

set -e

echo "🔄 Database Restore Script"
echo "=========================="
echo ""

# Configuration
DB_HOST="${DB_HOST:-db.ovlnwuvvegmcrrhwolgu.supabase.co}"
DB_PORT="${DB_PORT:-5432}"
DB_USER="${DB_USER:-postgres}"
DB_NAME="${DB_NAME:-postgres}"
BACKUP_DIR="${BACKUP_DIR:-./backups}"

# Find backup file
if [ -z "$1" ]; then
    echo "📁 Available backups:"
    ls -lh "$BACKUP_DIR"/backup_*.* 2>/dev/null | tail -5 || echo "   No backups found"
    echo ""
    echo "Usage: ./restore.sh <backup_file>"
    echo "Example: ./restore.sh backup_20240513_140530.dump.gz"
    exit 1
fi

BACKUP_FILE="$BACKUP_DIR/$1"

# Check if backup file exists
if [ ! -f "$BACKUP_FILE" ]; then
    echo "❌ Backup file not found: $BACKUP_FILE"
    exit 1
fi

echo "⚠️  WARNING: This will restore the database from backup"
echo "   File: $BACKUP_FILE"
echo "   Size: $(du -h "$BACKUP_FILE" | cut -f1)"
echo ""
read -p "Continue? (type 'yes' to confirm): " CONFIRM

if [ "$CONFIRM" != "yes" ]; then
    echo "❌ Cancelled"
    exit 0
fi

# Check if password is set
if [ -z "$DB_PASSWORD" ]; then
    echo "❌ Error: DB_PASSWORD environment variable not set"
    exit 1
fi

echo ""
echo "🔄 Restoring database..."

export PGPASSWORD="$DB_PASSWORD"

# Detect file format
if [[ "$BACKUP_FILE" == *.gz ]]; then
    # Compressed binary format
    echo "   Format: Compressed binary (dump)"
    gunzip -c "$BACKUP_FILE" | pg_restore \
        -h "$DB_HOST" \
        -p "$DB_PORT" \
        -U "$DB_USER" \
        -d "$DB_NAME" \
        -v \
        --no-owner \
        --no-privileges
elif [[ "$BACKUP_FILE" == *.sql ]]; then
    # SQL text format
    echo "   Format: SQL text"
    psql \
        -h "$DB_HOST" \
        -p "$DB_PORT" \
        -U "$DB_USER" \
        -d "$DB_NAME" \
        -f "$BACKUP_FILE" \
        -v ON_ERROR_STOP=1
else
    echo "❌ Unknown backup format"
    exit 1
fi

unset PGPASSWORD

if [ $? -eq 0 ]; then
    echo "   ✅ Restore completed successfully"
    
    # Log restore
    cat >> "$BACKUP_DIR/restore.log" << EOF
[$(date)] Restore completed
Backup File: $1
Status: SUCCESS

EOF
    echo ""
    echo "✅ Database restored successfully!"
else
    echo "   ❌ Restore failed!"
    exit 1
fi

echo ""
