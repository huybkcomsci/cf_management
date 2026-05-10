using CafeManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeManagement.Controllers;

[Authorize(Roles = "Admin,Kế toán")]
public class AdminController : Controller
{
    private readonly IReportService _reportService;
    private readonly ISalesService _salesService;

    public AdminController(IReportService reportService, ISalesService salesService)
    {
        _reportService = reportService;
        _salesService = salesService;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        var vm = await _reportService.GetAdminDashboardAsync(cancellationToken);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> DailySalesReport(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var report = await _salesService.GetDailySalesReportAsync(fromDate, toDate, cancellationToken);
        ViewBag.FromDate = fromDate;
        ViewBag.ToDate = toDate;
        return View(report);
    }
}

