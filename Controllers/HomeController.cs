using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CafeManagement.Models;
using CafeManagement.Data;
using Npgsql;

namespace CafeManagement.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IConfiguration configuration)
    {
        _logger = logger;
        _context = context;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CheckDatabaseConnection()
    {
        try
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return Json(new
                {
                    success = false,
                    message = "✗ Database connection failed",
                    details = "Connection string is empty"
                });
            }

            await using var dbConnection = new NpgsqlConnection(connectionString);
            await dbConnection.OpenAsync(timeoutCts.Token);
            await dbConnection.CloseAsync();

            var currentTime = DateTime.Now;

            return Json(new 
            { 
                success = true, 
                message = "✓ Database connection successful",
                details = $"Connected at: {currentTime:yyyy-MM-dd HH:mm:ss}",
                databaseTime = currentTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection check failed");
            return Json(new 
            { 
                success = false, 
                message = "✗ Database connection failed",
                details = ex.ToString(),
                type = ex.GetType().Name
            });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
