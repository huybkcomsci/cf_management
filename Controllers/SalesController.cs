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
    // [ValidateAntiForgeryToken]  // Temporary: disabled for debugging AJAX checkout
    public async Task<IActionResult> Checkout([FromBody] SalesCheckoutRequestViewModel request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            _logger.LogError("Checkout model validation failed: {Errors}", string.Join(", ", errors));
            return BadRequest(new { message = "Du lieu hoa don khong hop le", errors });
        }

        try
        {
            _logger.LogInformation("Checkout starting with {ItemCount} items", request.Items?.Count ?? 0);
            var result = await _salesService.CheckoutAsync(request, User.Identity?.Name, cancellationToken);
            _logger.LogInformation("Checkout succeeded: {MaHD}", result.MaHD);
            return Ok(result);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Checkout cancelled");
            return BadRequest(new { message = "Thanh toan bi huy" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Checkout error");
            return BadRequest(new { message = ex.Message, exceptionType = ex.GetType().Name });
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
    public async Task<IActionResult> History(DateTime? fromDate, DateTime? toDate, int take = 50, CancellationToken cancellationToken = default)
    {
        var invoices = await _salesService.GetInvoiceHistoryFilteredAsync(fromDate, toDate, take, cancellationToken);
        ViewBag.FromDate = fromDate;
        ViewBag.ToDate = toDate;
        ViewBag.Take = take;
        return View(invoices);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken = default)
    {
        var vm = await _salesService.GetInvoiceDetailsAsync(id, cancellationToken);
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
