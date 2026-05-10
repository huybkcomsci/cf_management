using CafeManagement.Data;
using CafeManagement.Models.Entities;
using CafeManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Controllers;

[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly ApplicationDbContext _db;

    public ProductsController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _db.Sanphams
            .AsNoTracking()
            .Include(s => s.NhomSP)
            .OrderBy(s => s.TenSP)
            .Select(s => new ProductListItemViewModel
            {
                Id = s.Id,
                TenSP = s.TenSP,
                TenNhom = s.NhomSP.TenNhom,
                GiaBan = s.GiaBan,
                SoLuongTon = s.SoLuongTon,
                DonViTinh = s.DonViTinh,
                IsActive = s.IsActive
            })
            .ToListAsync();
        return View(items);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var item = await _db.Sanphams
            .AsNoTracking()
            .Include(s => s.NhomSP)
            .Where(s => s.Id == id)
            .Select(s => new ProductListItemViewModel
            {
                Id = s.Id,
                TenSP = s.TenSP,
                TenNhom = s.NhomSP.TenNhom,
                GiaBan = s.GiaBan,
                SoLuongTon = s.SoLuongTon,
                DonViTinh = s.DonViTinh,
                IsActive = s.IsActive
            })
            .FirstOrDefaultAsync();
        if (item is null) return NotFound();
        return View(item);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _db.NhomSPs.ToListAsync();
        return View(new Sanpham { Id = Guid.NewGuid() });
    }

    [HttpPost]
    public async Task<IActionResult> Create(Sanpham model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _db.NhomSPs.ToListAsync();
            return View(model);
        }

        model.NgayTao = DateTime.UtcNow;
        model.NgayCapNhat = DateTime.UtcNow;
        _db.Sanphams.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _db.Sanphams.FindAsync(id);
        if (item is null) return NotFound();
        ViewBag.Categories = await _db.NhomSPs.ToListAsync();
        return View(item);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Sanpham model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _db.NhomSPs.ToListAsync();
            return View(model);
        }

        var exists = await _db.Sanphams.FindAsync(model.Id);
        if (exists is null) return NotFound();

        exists.TenSP = model.TenSP;
        exists.MoTa = model.MoTa;
        exists.GiaBan = model.GiaBan;
        exists.GiaNhap = model.GiaNhap;
        exists.SoLuongTon = model.SoLuongTon;
        exists.SLTonMin = model.SLTonMin;
        exists.DonViTinh = model.DonViTinh;
        exists.IsActive = model.IsActive;
        exists.IdNhom = model.IdNhom;
        exists.NgayCapNhat = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var item = await _db.Sanphams.FindAsync(id);
        if (item is null) return NotFound();
        _db.Sanphams.Remove(item);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
