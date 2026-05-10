using CafeManagement.Data;
using CafeManagement.Models;
using CafeManagement.Models.Entities;
using CafeManagement.Models.Enums;
using CafeManagement.Models.ViewModels;
using CafeManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Controllers;

[Authorize(Roles = "Admin,Kế toán")]
public class AdminController : Controller
{
    private readonly IReportService _reportService;
    private readonly ISalesService _salesService;
    private readonly ApplicationDbContext _db;

    public AdminController(IReportService reportService, ISalesService salesService, ApplicationDbContext db)
    {
        _reportService = reportService;
        _salesService = salesService;
        _db = db;
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

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Accounts(CancellationToken cancellationToken = default)
    {
        var vm = await BuildAccountsPageViewModelAsync(new AdminCreateAccountViewModel
        {
            Role = "Kế toán"
        }, cancellationToken);
        return View(vm);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAccount(AdminCreateAccountViewModel model, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View("Accounts", await BuildAccountsPageViewModelAsync(model, cancellationToken));
        }

        var email = model.Email.Trim();
        if (await _db.Users.AnyAsync(x => x.Email == email, cancellationToken))
        {
            ModelState.AddModelError(nameof(model.Email), "Email đã tồn tại");
            return View("Accounts", await BuildAccountsPageViewModelAsync(model, cancellationToken));
        }

        var now = DateTime.UtcNow;
        var user = new User
        {
            Email = email,
            Password = model.Password,
            Role = model.Role,
            IsActive = true,
            CreatedDate = now
        };

        _db.Users.Add(user);

        var employee = await _db.Nhanviens.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        if (employee is null)
        {
            _db.Nhanviens.Add(new Nhanvien
            {
                Id = Guid.NewGuid(),
                TenNV = model.EmployeeName.Trim(),
                Email = email,
                ChucVu = model.Role,
                TrangThai = TrangThaiNhanvien.DangLam,
                NgayVaoLam = now,
                NgayTao = now,
                NgayCapNhat = now
            });
        }
        else
        {
            employee.TenNV = model.EmployeeName.Trim();
            employee.Email = email;
            employee.ChucVu = model.Role;
            employee.TrangThai = TrangThaiNhanvien.DangLam;
            employee.NgayCapNhat = now;
        }

        await _db.SaveChangesAsync(cancellationToken);
        TempData["ToastType"] = "success";
        TempData["ToastMessage"] = "Tạo tài khoản thành công";
        return RedirectToAction(nameof(Accounts));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> ChangePassword(int id, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        return View(new AdminChangePasswordViewModel
        {
            UserId = user.Id,
            Email = user.Email
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(AdminChangePasswordViewModel model, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == model.UserId, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        user.Password = model.NewPassword;
        await _db.SaveChangesAsync(cancellationToken);

        TempData["ToastType"] = "success";
        TempData["ToastMessage"] = "Đổi mật khẩu thành công";
        return RedirectToAction(nameof(Accounts));
    }

    private async Task<AdminAccountsPageViewModel> BuildAccountsPageViewModelAsync(AdminCreateAccountViewModel createModel, CancellationToken cancellationToken)
    {
        var users = await _db.Users
            .AsNoTracking()
            .Where(x => x.Role == "Kế toán" || x.Role == "Thu ngân")
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);

        var emails = users.Select(x => x.Email).ToList();
        var employees = await _db.Nhanviens
            .AsNoTracking()
            .Where(x => x.Email != null && emails.Contains(x.Email))
            .Select(x => new { x.Email, x.TenNV })
            .ToListAsync(cancellationToken);

        var employeeMap = employees
            .Where(x => !string.IsNullOrWhiteSpace(x.Email))
            .ToDictionary(x => x.Email!, x => x.TenNV, StringComparer.OrdinalIgnoreCase);

        return new AdminAccountsPageViewModel
        {
            CreateAccount = createModel,
            Accounts = users.Select(x => new AdminUserAccountItemViewModel
            {
                Id = x.Id,
                Email = x.Email,
                Role = x.Role,
                IsActive = x.IsActive,
                CreatedDate = x.CreatedDate,
                EmployeeName = employeeMap.TryGetValue(x.Email, out var employeeName) ? employeeName : null
            }).ToList()
        };
    }
}

