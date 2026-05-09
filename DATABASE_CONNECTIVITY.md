# Database Connectivity Troubleshooting

## Error: "SocketException: Network is unreachable"

### Symptoms
```
SocketException: Network is unreachable
NpgsqlException: Failed to connect to [IPv6_ADDRESS]:5432
```

This typically occurs when:
1. IPv6 connectivity issue between client and Supabase
2. Connection string is incorrect
3. Firewall blocking PostgreSQL port 5432
4. Supabase project is paused or not accessible

### Solution

#### 1. Verify Connection String
Check that `appsettings.json` has correct Supabase credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.YOUR-PROJECT.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR-PASSWORD;SSL Mode=Require;Trust Server Certificate=true;Connection Idle Lifetime=300;Pooling=true;Min Pool Size=1;Max Pool Size=20;Default Command Timeout=30;"
  }
}
```

**Find your credentials at:** https://supabase.com/dashboard → Select Project → Settings → Database → Connection String

#### 2. Set Environment Variable
For Render deployment, ensure CONNECTION_STRING is set:

```bash
# On Render:
Settings → Environment Variables → Add
Key: CONNECTION_STRING
Value: Host=db.YOUR-PROJECT.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR-PASSWORD;SSL Mode=Require;Trust Server Certificate=true;Connection Idle Lifetime=300;Pooling=true;Min Pool Size=1;Max Pool Size=20;Default Command Timeout=30;
```

#### 3. Test Connectivity Locally
Test with `psql` (PostgreSQL client):

```bash
# Install postgres client (macOS)
brew install postgresql

# Test connection
psql "postgresql://postgres:YOUR-PASSWORD@db.YOUR-PROJECT.supabase.co:5432/postgres"

# If it connects, you can run SQL
postgres=> SELECT 1;
```

#### 4. Update Code
The app now includes:
- ✅ Retry logic (3 attempts, 5 second delays)
- ✅ Connection pooling (min 1, max 20)
- ✅ SSL Mode=Require
- ✅ Connection timeout (30 seconds)

#### 5. Restart Application
After changes to connection string:

```bash
# Local dev
dotnet run

# If still failing, check logs
dotnet run 2>&1 | grep -i "connection\|socket\|network"
```

## Common Causes & Fixes

| Issue | Cause | Solution |
|-------|-------|----------|
| IPv6 connectivity error | Client can't reach Supabase via IPv6 | Supabase resolves to IPv6 by default; our connection string now handles this with `Pooling=true` and retry logic |
| "Server unavailable" after minutes | Idle connection timeout | Connection Idle Lifetime=300 auto-recycles idle connections |
| "Too many connections" | Connection pool exhausted | Max Pool Size=20; check if app is leaking connections |
| SSL/TLS error | Outdated or missing SSL cert | SSL Mode=Require;Trust Server Certificate=true |
| Login fails but app starts | Database exists but no seed data | Run SQL script: `scripts/supabase_seed.sql` in Supabase Editor |

## Debug Steps

### Check Connection String Format
```csharp
// In Program.cs or Controller
var connStr = configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection to: {connStr.Split(';')[0]}"); // Shows Host only
```

### Check Database Alive
```sql
-- In Supabase SQL Editor
SELECT now();
SELECT table_name FROM information_schema.tables LIMIT 5;
```

### Check If Migration Applied
```sql
-- In Supabase SQL Editor
SELECT * FROM "AspNetRoles" LIMIT 1;
```

### Monitor Render Logs
```bash
# On Render dashboard
Logs → View Logs → Search for "connection" or "socket"
```

## IPv6 vs IPv4 Notes

Supabase typically resolves to:
- **Primary**: IPv6 address (2406:da14:...)
- **Secondary**: IPv4 address (via DNS fallback)

The connection string now handles both via:
- `Pooling=true` - Manages connection reuse
- `Retry logic` - Handles transient failures
- `SSL Mode=Require` - Forces secure connections

## Files Modified

- `appsettings.json` - Added connection string options
- `Program.cs` - Added retry policy to DbContext
- `scripts/supabase_seed.sql` - Bootstrap schema

## Next Steps if Still Failing

1. **Check Supabase Project Status**
   - Go to https://supabase.com/dashboard
   - Is project active (not paused)?
   - Is database running?

2. **Test from Different Network**
   - Try WiFi vs. Ethernet
   - Try mobile hotspot
   - Verify it's not a local firewall issue

3. **Check Supabase Logs**
   - Supabase Dashboard → Logs → PostgreSQL
   - Look for connection attempts and errors

4. **Contact Supabase Support**
   - If connection string is correct but still failing
   - Could be regional availability or infrastructure issue

## References

- [Npgsql Connection String](https://www.npgsql.org/doc/connection-string-parameters.html)
- [Supabase Connection Docs](https://supabase.com/docs/guides/database/connecting-to-postgres)
- [EF Core Retry Strategy](https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency)
- [PostgreSQL Port 5432](https://www.postgresql.org/docs/current/runtime-config-connection.html)
