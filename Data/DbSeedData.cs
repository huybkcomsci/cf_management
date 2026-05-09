using CafeManagement.Models.Entities;
using CafeManagement.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Data;

public static class DbSeedData
{
    public static async Task EnsureSeedDataAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();

        // Apply pending migrations so tables exist before seeding.
        await db.Database.MigrateAsync();

        var now = DateTime.UtcNow;

        var nhomCafe = await EnsureNhomAsync(db, "Ca phe", "Nhom ca phe nong/lanh", PhanLoai.DoUong, now);
        var nhomTraSua = await EnsureNhomAsync(db, "Tra sua", "Nhom do uong tra sua", PhanLoai.DoUong, now);
        var nhomBanh = await EnsureNhomAsync(db, "Banh ngot", "Nhom banh/an nhe", PhanLoai.ThucAn, now);
        var nhomNguyenLieu = await EnsureNhomAsync(db, "Nguyen lieu", "Nhom nguyen lieu pha che", PhanLoai.PhuLieu, now);

        var cafeDen = await EnsureSanphamAsync(db, nhomCafe.Id, "Ca phe den", "Ca phe den da", 30000m, 12000m, 120, 20, "ly", now);
        _ = cafeDen;
        var bacXiu = await EnsureSanphamAsync(db, nhomCafe.Id, "Bac xiu", "Bac xiu da", 35000m, 15000m, 100, 20, "ly", now);
        _ = bacXiu;
        var traSua = await EnsureSanphamAsync(db, nhomTraSua.Id, "Tra sua tran chau", "Tra sua truyen thong", 40000m, 18000m, 90, 15, "ly", now);
        _ = traSua;
        var tiramisu = await EnsureSanphamAsync(db, nhomBanh.Id, "Tiramisu", "Banh tiramisu mieng", 45000m, 22000m, 50, 10, "mieng", now);
        _ = tiramisu;

        // Ingredient stock items
        var cafeBot = await EnsureSanphamAsync(db, nhomNguyenLieu.Id, "Cafe bot", "Nguyen lieu pha cafe", 0m, 350m, 10000, 1000, "g", now);
        var suaDac = await EnsureSanphamAsync(db, nhomNguyenLieu.Id, "Sua dac", "Nguyen lieu sua dac", 0m, 200m, 8000, 800, "ml", now);

        // Sellable product with recipe
        var cafeSua = await EnsureSanphamAsync(db, nhomCafe.Id, "Cafe sua", "Cafe sua da", 38000m, 16000m, 9999, 0, "ly", now);

        await EnsureDinhluongAsync(db, cafeSua.Id, cafeBot.Id, 20, "g", "1 ly cafe sua dung 20g cafe", now);
        await EnsureDinhluongAsync(db, cafeSua.Id, suaDac.Id, 30, "ml", "1 ly cafe sua dung 30ml sua", now);
    }

    private static async Task<NhomSP> EnsureNhomAsync(
        ApplicationDbContext db,
        string tenNhom,
        string? moTa,
        PhanLoai phanLoai,
        DateTime now)
    {
        var nhom = await db.NhomSPs.FirstOrDefaultAsync(x => x.TenNhom == tenNhom);
        if (nhom is not null)
        {
            return nhom;
        }

        nhom = new NhomSP
        {
            Id = Guid.NewGuid(),
            TenNhom = tenNhom,
            MoTa = moTa,
            PhanLoai = phanLoai,
            NgayTao = now,
            NgayCapNhat = now
        };
        await db.NhomSPs.AddAsync(nhom);
        await db.SaveChangesAsync();
        return nhom;
    }

    private static async Task<Sanpham> EnsureSanphamAsync(
        ApplicationDbContext db,
        Guid idNhom,
        string ten,
        string? moTa,
        decimal giaBan,
        decimal giaNhap,
        int ton,
        int tonMin,
        string donVi,
        DateTime now)
    {
        var sp = await db.Sanphams.FirstOrDefaultAsync(x => x.TenSP == ten);
        if (sp is not null)
        {
            return sp;
        }

        sp = new Sanpham
        {
            Id = Guid.NewGuid(),
            IdNhom = idNhom,
            TenSP = ten,
            MoTa = moTa,
            GiaBan = giaBan,
            GiaNhap = giaNhap,
            SoLuongTon = ton,
            SLTonMin = tonMin,
            DonViTinh = donVi,
            IsActive = true,
            NgayTao = now,
            NgayCapNhat = now
        };
        await db.Sanphams.AddAsync(sp);
        await db.SaveChangesAsync();
        return sp;
    }

    private static async Task EnsureDinhluongAsync(
        ApplicationDbContext db,
        Guid idSp,
        Guid idThanhPhan,
        int soLuong,
        string donVi,
        string? ghiChu,
        DateTime now)
    {
        var existed = await db.Dinhluongs.AnyAsync(x => x.IdSP == idSp && x.IdThanhPhan == idThanhPhan);
        if (existed)
        {
            return;
        }

        await db.Dinhluongs.AddAsync(new Dinhluong
        {
            Id = Guid.NewGuid(),
            IdSP = idSp,
            IdThanhPhan = idThanhPhan,
            SoLuong = soLuong,
            DonVi = donVi,
            GhiChu = ghiChu,
            NgayTao = now,
            NgayCapNhat = now
        });
        await db.SaveChangesAsync();
    }
}
