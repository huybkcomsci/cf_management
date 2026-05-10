using CafeManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!DemoUsers.TryGetValue(model.Email.Trim(), out var demoUser) || demoUser.Password != model.Password)
        {
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, model.Email.Trim()),
            new(ClaimTypes.Email, model.Email.Trim()),
            new(ClaimTypes.Role, demoUser.Role)
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