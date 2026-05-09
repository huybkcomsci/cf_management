using CafeManagement.Data;
using CafeManagement.Models.Entities;
using CafeManagement.Models.Enums;
using CafeManagement.Models.ViewModels;
using CafeManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Services;

public class HrService : IHrService
{
    private readonly ApplicationDbContext _db;
    private const double StandardHoursPerDay = 8.0;
    private const decimal OvertimeRateMultiplier = 1.5m;

    public HrService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task CheckInAsync(Guid nhanvienId, DateTime? ngay = null, CancellationToken cancellationToken = default)
    {
        var date = (ngay ?? DateTime.UtcNow).Date;
        var record = await _db.Chamcongs.FirstOrDefaultAsync(x => x.IdNV == nhanvienId && x.Ngay.Date == date, cancellationToken);
        if (record is null)
        {
            record = new Chamcong
            {
                Id = Guid.NewGuid(),
                IdNV = nhanvienId,
                Ngay = date,
                GioVao = DateTime.UtcNow.TimeOfDay,
                TrangThai = TrangThaiChamCong.CoMat,
                NgayTao = DateTime.UtcNow
            };
            await _db.Chamcongs.AddAsync(record, cancellationToken);
        }
        else if (record.GioVao is null)
        {
            record.GioVao = DateTime.UtcNow.TimeOfDay;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task CheckOutAsync(Guid nhanvienId, DateTime? ngay = null, CancellationToken cancellationToken = default)
    {
        var date = (ngay ?? DateTime.UtcNow).Date;
        var record = await _db.Chamcongs.FirstOrDefaultAsync(x => x.IdNV == nhanvienId && x.Ngay.Date == date, cancellationToken);
        if (record is null)
        {
            throw new InvalidOperationException("Nhan vien chua check-in");
        }

        record.GioRa = DateTime.UtcNow.TimeOfDay;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AttendanceDashboardRowViewModel>> GetAttendanceAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        var records = await _db.Chamcongs
            .AsNoTracking()
            .Include(x => x.Nhanvien)
            .Where(x => x.Ngay >= fromDate.Date && x.Ngay <= toDate.Date)
            .OrderByDescending(x => x.Ngay)
            .ToListAsync(cancellationToken);

        return records.Select(r =>
        {
            var total = CalculateHours(r.GioVao, r.GioRa);
            return new AttendanceDashboardRowViewModel
            {
                NhanvienId = r.IdNV,
                TenNhanvien = r.Nhanvien.TenNV,
                Ngay = r.Ngay,
                GioVao = r.GioVao,
                GioRa = r.GioRa,
                TongGio = total,
                OvertimeGio = Math.Max(0, total - StandardHoursPerDay)
            };
        }).ToList();
    }

    public async Task<IReadOnlyList<PayrollResultViewModel>> CalculatePayrollAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1);

        var query = await _db.Chamcongs
            .AsNoTracking()
            .Include(x => x.Nhanvien)
            .Where(x => x.Ngay >= from && x.Ngay < to)
            .ToListAsync(cancellationToken);

        var grouped = query.GroupBy(x => new { x.IdNV, x.Nhanvien.TenNV, x.Nhanvien.Luong });

        return grouped.Select(g =>
        {
            var totalHours = g.Sum(x => CalculateHours(x.GioVao, x.GioRa));
            var overtime = g.Sum(x => Math.Max(0, CalculateHours(x.GioVao, x.GioRa) - StandardHoursPerDay));
            var baseSalary = g.Key.Luong ?? 0m;
            var hourlyRate = baseSalary / 26m / (decimal)StandardHoursPerDay;
            var overtimeSalary = (decimal)overtime * hourlyRate * OvertimeRateMultiplier;
            var totalSalary = baseSalary + overtimeSalary;

            return new PayrollResultViewModel
            {
                NhanvienId = g.Key.IdNV,
                TenNhanvien = g.Key.TenNV,
                Year = year,
                Month = month,
                SoGio = (int)Math.Round(totalHours),
                LuongCoBan = baseSalary,
                LuongOvertime = overtimeSalary,
                TongLuong = totalSalary
            };
        }).ToList();
    }

    public async Task SavePayrollAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        var payroll = await CalculatePayrollAsync(year, month, cancellationToken);
        foreach (var p in payroll)
        {
            var existing = await _db.Bangluongs
                .FirstOrDefaultAsync(x => x.IdNV == p.NhanvienId && x.Nam == year && x.Thang == month, cancellationToken);

            if (existing is null)
            {
                await _db.Bangluongs.AddAsync(new Bangluong
                {
                    Id = Guid.NewGuid(),
                    IdNV = p.NhanvienId,
                    Nam = year,
                    Thang = month,
                    SoGio = p.SoGio,
                    Luong = p.LuongCoBan,
                    PhuCap = p.LuongOvertime,
                    KhauTru = 0,
                    TongLuong = p.TongLuong,
                    TrangThai = TrangThai.PheDuyet,
                    NgayTao = DateTime.UtcNow,
                    NgayCapNhat = DateTime.UtcNow
                }, cancellationToken);
            }
            else
            {
                existing.SoGio = p.SoGio;
                existing.Luong = p.LuongCoBan;
                existing.PhuCap = p.LuongOvertime;
                existing.TongLuong = p.TongLuong;
                existing.NgayCapNhat = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private static double CalculateHours(TimeSpan? checkIn, TimeSpan? checkOut)
    {
        if (checkIn is null || checkOut is null || checkOut <= checkIn)
        {
            return 0;
        }

        return (checkOut.Value - checkIn.Value).TotalHours;
    }
}
