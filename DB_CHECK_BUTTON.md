# Database Connection Checker

## Feature Overview

A simple, user-friendly button on the home page that checks database connectivity in real-time.

## How It Works

### Frontend (Client-Side)
- **Location**: Home page `/`
- **Button**: "Check Database Connection"
- **Action**: Sends POST request to backend
- **Display**: 
  - Shows loading spinner while checking
  - Displays success (green) or error (red) status
  - Shows detailed error messages if connection fails

### Backend (Server-Side)
- **Endpoint**: `POST /Home/CheckDatabaseConnection`
- **Logic**:
  1. Calls `_context.Database.CanConnectAsync()` to verify connection
  2. On success: Returns timestamp
  3. On failure: Returns error message with exception details
- **Error Handling**: Catches and logs all exceptions

## Usage

### For Users
1. Go to application home page
2. Look for "Check Database Connection" button
3. Click the button
4. Wait for result (shows loading spinner)
5. Read status message:
   - ✓ Green = Database is connected and working
   - ✗ Red = Database connection failed

### Example Results

**Success Response:**
```
✓ Database connection successful
Connected at: 2026-05-09 14:30:45
Database time: 2026-05-09 14:30:45
```

**Error Response (Network Issue):**
```
✗ Database connection failed
Network is unreachable
Error type: SocketException
```

**Error Response (Invalid Credentials):**
```
✗ Database connection failed
FATAL: password authentication failed for user "postgres"
Error type: NpgsqlException
```

## Troubleshooting with This Feature

### Scenario 1: "Network is unreachable"
- **Cause**: IPv6 connectivity issue (Supabase → local/Render)
- **Action**: Check [DATABASE_CONNECTIVITY.md](DATABASE_CONNECTIVITY.md)

### Scenario 2: "password authentication failed"
- **Cause**: Wrong password or username in connection string
- **Action**: Verify CONNECTION_STRING environment variable

### Scenario 3: "timeout"
- **Cause**: Database is slow or unreachable
- **Action**: Check Supabase dashboard, verify network

### Scenario 4: Shows success but login fails
- **Cause**: Database connected but no seed data or Identity tables missing
- **Action**: Run SQL script: `scripts/supabase_seed.sql` in Supabase

## Technical Details

### Controller Code
```csharp
[HttpPost]
public async Task<IActionResult> CheckDatabaseConnection()
{
    try
    {
        var canConnect = await _context.Database.CanConnectAsync();
        
        if (canConnect)
        {
            return Json(new 
            { 
                success = true, 
                message = "✓ Database connection successful",
                details = $"Connected at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                databaseTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
        else
        {
            return Json(new 
            { 
                success = false, 
                message = "✗ Cannot connect to database",
                details = "CanConnectAsync returned false"
            });
        }
    }
    catch (Exception ex)
    {
        _logger.LogError($"Database connection check failed: {ex.Message}");
        return Json(new 
        { 
            success = false, 
            message = "✗ Database connection failed",
            details = ex.Message,
            type = ex.GetType().Name
        });
    }
}
```

### View Code
- **File**: `Views/Home/Index.cshtml`
- **Features**:
  - Bootstrap button styling
  - Loading spinner during check
  - Color-coded alert (green/red)
  - JavaScript AJAX request
  - Handles errors gracefully

## Integration Points

- **Requires**: ApplicationDbContext (dependency injection in HomeController)
- **Logging**: Uses ILogger<HomeController> to log errors
- **Security**: POST request (CSRF protection via ASP.NET Core)
- **Performance**: Non-blocking async/await

## Files Modified

- `Controllers/HomeController.cs` - Added CheckDatabaseConnection action
- `Views/Home/Index.cshtml` - Added button and JavaScript

## Deployment Notes

- **Local Development**: Works immediately after connection string is configured
- **Render**: Will show actual database issues if connection string is incorrect
- **Testing**: Use this button to verify deployment before going to production

## Future Enhancements

Possible improvements:
- [ ] Add query response time measurement
- [ ] Check specific tables (AspNetUsers, Sanpham, etc.)
- [ ] Add authentication requirement (admin only)
- [ ] Log check history to database
- [ ] Health check endpoint for monitoring services

## References

- [EF Core Database API](https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.infrastructure.databasefacade.canconnectasync)
- [Bootstrap Alerts](https://getbootstrap.com/docs/5.0/components/alerts/)
- [DOM Manipulation with JavaScript](https://developer.mozilla.org/en-US/docs/Web/API/Document)
