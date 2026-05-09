using CafeManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeManagement.Controllers;

[Authorize(Roles = "Admin,Kế toán")]
public class ReportsController : Controller
{
    private readonly IReportService _reportService;
    private readonly IExportService _exportService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IReportService reportService, IExportService exportService, ILogger<ReportsController> logger)
    {
        _reportService = reportService;
        _exportService = exportService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Revenue(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken)
    {
        var vm = await _reportService.GetRevenueSummaryAsync(fromDate, toDate, cancellationToken);
        ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
        ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Inventory(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken)
    {
        var vm = await _reportService.GetInventoryReportAsync(fromDate, toDate, cancellationToken);
        ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
        ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> ExportInventoryExcel(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken)
    {
        try
        {
            var file = await _exportService.ExportInventoryReportToExcelAsync(cancellationToken);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"bao-cao-ton-kho_{DateTime.Now:yyyyMMdd}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting inventory to Excel");
            TempData["Error"] = "Lỗi xuất báo cáo tồn kho";
            return RedirectToAction(nameof(Inventory));
        }
    }

    [HttpGet]
    public async Task<IActionResult> ExportInventoryPdf(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken)
    {
        try
        {
            var file = await _exportService.ExportInventoryReportToPdfAsync(cancellationToken);
            return File(file, "application/pdf", $"bao-cao-ton-kho_{DateTime.Now:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting inventory to PDF");
            TempData["Error"] = "Lỗi xuất báo cáo tồn kho";
            return RedirectToAction(nameof(Inventory));
        }
    }

    [HttpGet]
    public async Task<IActionResult> ExportRevenueExcel(DateTime? fromDate, DateTime? toDate, string reportType = "daily", CancellationToken cancellationToken = default)
    {
        try
        {
            var from = fromDate ?? DateTime.Now.AddMonths(-1);
            var to = toDate ?? DateTime.Now;

            var file = await _exportService.ExportRevenueReportToExcelAsync(from, to, reportType, cancellationToken);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"bao-cao-doanh-thu_{DateTime.Now:yyyyMMdd}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting revenue to Excel");
            TempData["Error"] = "Lỗi xuất báo cáo doanh thu";
            return RedirectToAction(nameof(Revenue));
        }
    }

    [HttpGet]
    public async Task<IActionResult> ExportRevenuePdf(DateTime? fromDate, DateTime? toDate, string reportType = "daily", CancellationToken cancellationToken = default)
    {
        try
        {
            var from = fromDate ?? DateTime.Now.AddMonths(-1);
            var to = toDate ?? DateTime.Now;

            var file = await _exportService.ExportRevenueReportToPdfAsync(from, to, reportType, cancellationToken);
            return File(file, "application/pdf", $"bao-cao-doanh-thu_{DateTime.Now:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting revenue to PDF");
            TempData["Error"] = "Lỗi xuất báo cáo doanh thu";
            return RedirectToAction(nameof(Revenue));
        }
    }
}
