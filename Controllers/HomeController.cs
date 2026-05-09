using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CafeManagement.Models;
using CafeManagement.Data;

namespace CafeManagement.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
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
            // Try to connect to database
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (canConnect)
            {
                // Get current database time
                var currentTime = DateTime.Now;
                
                return Json(new 
                { 
                    success = true, 
                    message = "✓ Database connection successful",
                    details = $"Connected at: {currentTime:yyyy-MM-dd HH:mm:ss}",
                    databaseTime = currentTime.ToString("yyyy-MM-dd HH:mm:ss")
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
