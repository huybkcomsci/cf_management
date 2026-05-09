using CafeManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeManagement.Controllers;

[Authorize(Roles = "Admin,Kế toán")]
public class HrController : Controller
{
    private readonly IHrService _hrService;

    public HrController(IHrService hrService)
    {
        _hrService = hrService;
    }

    [HttpGet]
    public async Task<IActionResult> Attendance(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken)
    {
        var from = fromDate?.Date ?? DateTime.UtcNow.Date.AddDays(-7);
        var to = toDate?.Date ?? DateTime.UtcNow.Date;
        var data = await _hrService.GetAttendanceAsync(from, to, cancellationToken);

        ViewBag.FromDate = from.ToString("yyyy-MM-dd");
        ViewBag.ToDate = to.ToString("yyyy-MM-dd");
        return View(data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckIn(Guid nhanvienId, CancellationToken cancellationToken)
    {
        await _hrService.CheckInAsync(nhanvienId, null, cancellationToken);
        TempData["ToastType"] = "success";
        TempData["ToastMessage"] = "Check-in thanh cong";
        return RedirectToAction(nameof(Attendance));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckOut(Guid nhanvienId, CancellationToken cancellationToken)
    {
        await _hrService.CheckOutAsync(nhanvienId, null, cancellationToken);
        TempData["ToastType"] = "success";
        TempData["ToastMessage"] = "Check-out thanh cong";
        return RedirectToAction(nameof(Attendance));
    }

    [HttpGet]
    public async Task<IActionResult> Payroll(int? year, int? month, CancellationToken cancellationToken)
    {
        var y = year ?? DateTime.UtcNow.Year;
        var m = month ?? DateTime.UtcNow.Month;
        var data = await _hrService.CalculatePayrollAsync(y, m, cancellationToken);

        ViewBag.Year = y;
        ViewBag.Month = m;
        return View(data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePayroll(int year, int month, CancellationToken cancellationToken)
    {
        await _hrService.SavePayrollAsync(year, month, cancellationToken);
        TempData["ToastType"] = "success";
        TempData["ToastMessage"] = "Da luu bang luong";
        return RedirectToAction(nameof(Payroll), new { year, month });
    }
}
