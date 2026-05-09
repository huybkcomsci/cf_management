using CafeManagement.Models.ViewModels;
using CafeManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeManagement.Controllers;

[Authorize(Roles = "Admin,Kế toán,Thu ngân")]
public class SalesController : Controller
{
    private readonly ISalesService _salesService;
    private readonly IExportService _exportService;
    private readonly ILogger<SalesController> _logger;

    public SalesController(ISalesService salesService, IExportService exportService, ILogger<SalesController> logger)
    {
        _salesService = salesService;
        _exportService = exportService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? q, CancellationToken cancellationToken)
    {
        var menu = await _salesService.GetMenuAsync(q, cancellationToken);
        return View(new SalesPageViewModel { MenuItems = menu });
    }

    [HttpGet]
    public async Task<IActionResult> GetMenu(string? q, CancellationToken cancellationToken)
    {
        var menu = await _salesService.GetMenuAsync(q, cancellationToken);
        return Json(menu);
    }

    [HttpGet]
    public async Task<IActionResult> GetProduct(Guid id, CancellationToken cancellationToken)
    {
        var product = await _salesService.GetProductAsync(id, cancellationToken);
        if (product is null)
        {
            return NotFound();
        }

        return Json(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout([FromBody] SalesCheckoutRequestViewModel request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Du lieu hoa don khong hop le" });
        }

        try
        {
            var result = await _salesService.CheckoutAsync(request, User.Identity?.Name, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Print(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _salesService.GetPrintDataAsync(id, cancellationToken);
        if (vm is null)
        {
            return NotFound();
        }

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> ExportInvoicePdf(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var file = await _exportService.ExportInvoiceToPdfAsync(id, cancellationToken);
            return File(file, "application/pdf", $"hoa-don_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting invoice to PDF");
            TempData["Error"] = "Lỗi xuất hóa đơn PDF";
            return RedirectToAction(nameof(Print), new { id });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ExportInvoiceExcel(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var file = await _exportService.ExportInvoiceToExcelAsync(id, cancellationToken);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"hoa-don_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting invoice to Excel");
            TempData["Error"] = "Lỗi xuất hóa đơn Excel";
            return RedirectToAction(nameof(Print), new { id });
        }
    }
}
