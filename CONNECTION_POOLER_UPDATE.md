# Supabase Connection String Update - Transaction Pooler (IPv4)

## New Connection Details

### Pooler Endpoint (Recommended for Render & Production)
```
PostgreSQL Connection String:
postgresql://postgres.ovlnwuvvegmcrrhwolgu:YOUR-PASSWORD@aws-1-ap-northeast-1.pooler.supabase.com:6543/postgres
```

### ASP.NET Core Connection String
```
Host=aws-1-ap-northeast-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.ovlnwuvvegmcrrhwolgu;Password=YOUR-PASSWORD;SSL Mode=Require;Trust Server Certificate=true;Connection Idle Lifetime=300;Pooling=true;Min Pool Size=1;Max Pool Size=20;Default Command Timeout=30;
```

## Key Differences

| Aspect | Old Direct | New Pooler |
|--------|-----------|-----------|
| **Host** | db.ovlnwuvvegmcrrhwolgu.supabase.co | aws-1-ap-northeast-1.pooler.supabase.com |
| **Port** | 5432 | 6543 |
| **Username** | postgres | postgres.ovlnwuvvegmcrrhwolgu |
| **IPv6 Compatible** | Yes (can fail) | No (IPv4 only) |
| **Connection Type** | Direct | Transaction Pooler |
| **Best For** | Direct connections | App servers, Render, AWS Lambda |

## Why Transaction Pooler?

✅ **Advantages:**
- IPv4 only → Eliminates IPv6 connectivity issues
- Connection pooling built-in → Better resource usage
- Lower latency from AWS ap-northeast-1 region
- Perfect for Render, serverless, and cloud deployments
- Better handles concurrent connections
- Lower memory footprint

❌ **Limitations:**
- Session-level features not supported (connections reused)
- Still supports: transactions, roles, prepared statements

## Files Updated

1. **appsettings.json** - Development connection string
2. **appsettings.Production.json** - Production connection string  
3. **DATABASE_CONNECTIVITY.md** - Documentation with new format

## How to Configure

### Local Development
1. Open `appsettings.json`
2. Replace `YOUR-PASSWORD` with actual Supabase password
3. Connection string should look like:
   ```
   Host=aws-1-ap-northeast-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.ovlnwuvvegmcrrhwolgu;Password=YourActualPassword123;...
   ```
4. Run: `dotnet run`

### Render Production
1. Go to Render Dashboard
2. Select Service → Settings
3. Add Environment Variable:
   - **Key**: `CONNECTION_STRING`
   - **Value**: `Host=aws-1-ap-northeast-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.ovlnwuvvegmcrrhwolgu;Password=YOUR-PASSWORD;SSL Mode=Require;Trust Server Certificate=true;Connection Idle Lifetime=300;Pooling=true;Min Pool Size=1;Max Pool Size=20;Default Command Timeout=30;`
4. Deploy

## Testing Connection

Use the built-in Database Connection Checker:
1. Go to application home page (`/`)
2. Click "Check Database Connection" button
3. See result:
   - ✓ Green = Connected successfully
   - ✗ Red = Connection failed (check error details)

## Troubleshooting

### "Network is unreachable"
- **Before**: Common with IPv6
- **Now**: Should be resolved with pooler (IPv4 only)
- If still occurring: Check firewall/security groups

### "Connection refused"
- Verify pooler endpoint is correct
- Check password is accurate
- Verify Supabase project is active

### "Too many connections"
- Pooler handles this automatically
- Connection Idle Lifetime=300 recycles idle connections
- Max Pool Size=20 limits concurrent connections

## Supabase Data Sources

To view your connection details anytime:
1. Go to https://supabase.com/dashboard
2. Select your project
3. Click **Settings** (bottom left)
4. Click **Database**
5. Look for **"Connection Pooler"** section
6. Select **"IPv4"** pooler (Transaction Pooler)

## References

- [Supabase Connection Pooler Docs](https://supabase.com/docs/guides/database/connecting-to-postgres#connection-pooler)
- [Npgsql Connection String](https://www.npgsql.org/doc/connection-string-parameters.html)
- [Our Database Connectivity Guide](DATABASE_CONNECTIVITY.md)
- [Database Check Button](DB_CHECK_BUTTON.md)

## Commit

```
1257d78 Update connection strings to use Supabase Transaction Pooler (IPv4 compatible)
```
