using CafeManagement.Data;
using CafeManagement.Models.Entities;
using CafeManagement.Models.ViewModels;
using CafeManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Services;

public class DinhluongService : IDinhluongService
{
    private readonly ApplicationDbContext _db;

    public DinhluongService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<DinhluongListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Dinhluongs
            .AsNoTracking()
            .Include(x => x.Sanpham)
            .Include(x => x.ThanhphanSanpham)
            .OrderBy(x => x.Sanpham.TenSP)
            .ThenBy(x => x.ThanhphanSanpham.TenSP)
            .Select(x => new DinhluongListItemViewModel
            {
                Id = x.Id,
                IdSP = x.IdSP,
                IdThanhPhan = x.IdThanhPhan,
                Sanpham = x.Sanpham.TenSP,
                Thanhphan = x.ThanhphanSanpham.TenSP,
                SoLuong = x.SoLuong,
                DonVi = x.DonVi,
                GhiChu = x.GhiChu
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<DinhluongEditViewModel> BuildCreateModelAsync(CancellationToken cancellationToken = default)
    {
        return new DinhluongEditViewModel
        {
            SanphamOptions = await BuildProductOptionsAsync(cancellationToken),
            ThanhphanOptions = await BuildProductOptionsAsync(cancellationToken)
        };
    }

    public async Task<DinhluongEditViewModel?> BuildEditModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var row = await _db.Dinhluongs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (row is null)
        {
            return null;
        }

        return new DinhluongEditViewModel
        {
            Id = row.Id,
            IdSP = row.IdSP,
            IdThanhPhan = row.IdThanhPhan,
            SoLuong = row.SoLuong,
            DonVi = row.DonVi,
            GhiChu = row.GhiChu,
            SanphamOptions = await BuildProductOptionsAsync(cancellationToken),
            ThanhphanOptions = await BuildProductOptionsAsync(cancellationToken)
        };
    }

    public async Task<Guid> CreateAsync(DinhluongEditViewModel model, CancellationToken cancellationToken = default)
    {
        await ValidateBusinessRuleAsync(model.IdSP, model.IdThanhPhan, null, cancellationToken);

        var now = DateTime.UtcNow;
        var entity = new Dinhluong
        {
            Id = Guid.NewGuid(),
            IdSP = model.IdSP,
            IdThanhPhan = model.IdThanhPhan,
            SoLuong = model.SoLuong,
            DonVi = model.DonVi,
            GhiChu = model.GhiChu,
            NgayTao = now,
            NgayCapNhat = now
        };

        await _db.Dinhluongs.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, DinhluongEditViewModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Dinhluongs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        await ValidateBusinessRuleAsync(model.IdSP, model.IdThanhPhan, id, cancellationToken);

        entity.IdSP = model.IdSP;
        entity.IdThanhPhan = model.IdThanhPhan;
        entity.SoLuong = model.SoLuong;
        entity.DonVi = model.DonVi;
        entity.GhiChu = model.GhiChu;
        entity.NgayCapNhat = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Dinhluongs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _db.Dinhluongs.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<DinhluongListItemViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Dinhluongs
            .AsNoTracking()
            .Include(x => x.Sanpham)
            .Include(x => x.ThanhphanSanpham)
            .Where(x => x.Id == id)
            .Select(x => new DinhluongListItemViewModel
            {
                Id = x.Id,
                IdSP = x.IdSP,
                IdThanhPhan = x.IdThanhPhan,
                Sanpham = x.Sanpham.TenSP,
                Thanhphan = x.ThanhphanSanpham.TenSP,
                SoLuong = x.SoLuong,
                DonVi = x.DonVi,
                GhiChu = x.GhiChu
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<List<SelectListItem>> BuildProductOptionsAsync(CancellationToken cancellationToken)
    {
        return await _db.Sanphams
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.TenSP)
            .Select(x => new SelectListItem(x.TenSP, x.Id.ToString()))
            .ToListAsync(cancellationToken);
    }

    private async Task ValidateBusinessRuleAsync(Guid idSp, Guid idThanhPhan, Guid? currentId, CancellationToken cancellationToken)
    {
        if (idSp == idThanhPhan)
        {
            throw new InvalidOperationException("San pham va nguyen lieu khong duoc trung nhau");
        }

        var duplicate = await _db.Dinhluongs.AnyAsync(
            x => x.IdSP == idSp && x.IdThanhPhan == idThanhPhan && (!currentId.HasValue || x.Id != currentId.Value),
            cancellationToken);

        if (duplicate)
        {
            throw new InvalidOperationException("Cong thuc nay da ton tai");
        }
    }
}
