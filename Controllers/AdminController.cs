using CafeManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeManagement.Controllers;

[Authorize(Roles = "Admin,Kế toán")]
public class AdminController : Controller
{
    private readonly IReportService _reportService;

    public AdminController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        var vm = await _reportService.GetAdminDashboardAsync(cancellationToken);
        return View(vm);
    }
}
