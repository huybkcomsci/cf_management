using CafeManagement.DTOs;
using CafeManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CafeManagement.Controllers;

[Authorize(Roles = "Admin,Kế toán,Thu ngân")]
public class SanphamController : Controller
{
    private readonly ISanphamService _sanphamService;

    public SanphamController(ISanphamService sanphamService)
    {
        _sanphamService = sanphamService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? q, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var vm = await _sanphamService.GetPagedAsync(q, page, pageSize, cancellationToken);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var item = await _sanphamService.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Kế toán")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await LoadNhomSelectList(cancellationToken);
        return View(new SanphamUpsertDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Kế toán")]
    public async Task<IActionResult> Create(SanphamUpsertDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadNhomSelectList(cancellationToken, dto.IdNhom);
            return View(dto);
        }

        await _sanphamService.CreateAsync(dto, cancellationToken);
        TempData["ToastType"] = "success";
        TempData["ToastMessage"] = "Them san pham thanh cong";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Kế toán")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var item = await _sanphamService.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        var dto = new SanphamUpsertDto
        {
            Id = item.Id,
            IdNhom = item.IdNhom,
            TenSP = item.TenSP,
            MoTa = item.MoTa,
            GiaBan = item.GiaBan,
            GiaNhap = item.GiaNhap,
            SoLuongTon = item.SoLuongTon,
            SLTonMin = item.SLTonMin,
            DonViTinh = item.DonViTinh,
            IsActive = item.IsActive
        };

        await LoadNhomSelectList(cancellationToken, dto.IdNhom);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Kế toán")]
    public async Task<IActionResult> Edit(Guid id, SanphamUpsertDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadNhomSelectList(cancellationToken, dto.IdNhom);
            return View(dto);
        }

        var ok = await _sanphamService.UpdateAsync(id, dto, cancellationToken);
        if (!ok)
        {
            return NotFound();
        }

        TempData["ToastType"] = "success";
        TempData["ToastMessage"] = "Cap nhat san pham thanh cong";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var item = await _sanphamService.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var ok = await _sanphamService.DeleteAsync(id, cancellationToken);
        if (!ok)
        {
            return NotFound();
        }

        TempData["ToastType"] = "success";
        TempData["ToastMessage"] = "Xoa san pham thanh cong";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadNhomSelectList(CancellationToken cancellationToken, Guid? selected = null)
    {
        var options = await _sanphamService.GetNhomOptionsAsync(cancellationToken);
        ViewBag.NhomOptions = new SelectList(options, nameof(NhomOptionDto.Id), nameof(NhomOptionDto.TenNhom), selected);
    }
}
