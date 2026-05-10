using CafeManagement.Data;
using CafeManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CafeManagement.Controllers;

public class AccountController : Controller
{
    private static readonly Dictionary<string, (string Password, string Role)> DemoUsers = new(StringComparer.OrdinalIgnoreCase)
    {
        ["admin@cafemanagement.local"] = ("Admin@123", "Admin"),
        ["keytoan@cafemanagement.local"] = ("Admin@123", "Kế toán"),
        ["thunga@cafemanagement.local"] = ("Admin@123", "Thu ngân")
    };

    private readonly ApplicationDbContext _db;

    public AccountController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.Email.Trim();
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email && x.IsActive, cancellationToken);

        string role;
        if (user is not null)
        {
            if (!string.Equals(user.Password, model.Password, StringComparison.Ordinal))
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng");
                return View(model);
            }

            role = user.Role;
        }
        else if (DemoUsers.TryGetValue(email, out var demoUser))
        {
            if (!string.Equals(demoUser.Password, model.Password, StringComparison.Ordinal))
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng");
                return View(model);
            }

            role = demoUser.Role;
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, email),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        TempData["ToastType"] = "success";
        TempData["ToastMessage"] = "Đăng nhập thành công";

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["ToastType"] = "info";
        TempData["ToastMessage"] = "Đã đăng xuất";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}