using CafeManagement.Data;
using CafeManagement.Models.Enums;
using CafeManagement.Models.ViewModels;
using CafeManagement.Services.Interfaces;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CafeManagement.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _db;

    public ReportService(ApplicationDbContext db)
    {
        _db = db;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<RevenueSummaryViewModel> GetRevenueSummaryAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var from = fromDate?.Date ?? DateTime.UtcNow.Date.AddDays(-30);
        var to = (toDate?.Date ?? DateTime.UtcNow.Date).AddDays(1);

        var invoicesQuery = _db.Hoadons
            .AsNoTracking()
            .Where(x => x.NgayLapHD >= from && x.NgayLapHD < to && x.TrangThai == TrangThai.HoanThanh);

        var totalRevenue = await invoicesQuery.SumAsync(x => (decimal?)x.TongCong, cancellationToken) ?? 0;
        var invoiceCount = await invoicesQuery.CountAsync(cancellationToken);

        var dailySeries = await invoicesQuery
            .GroupBy(x => x.NgayLapHD.Date)
            .Select(g => new RevenuePointViewModel
            {
                Label = g.Key.ToString("dd/MM"),
                Value = g.Sum(x => x.TongCong)
            })
            .OrderBy(x => x.Label)
            .ToListAsync(cancellationToken);

        var monthlySeries = await invoicesQuery
            .GroupBy(x => new { x.NgayLapHD.Year, x.NgayLapHD.Month })
            .Select(g => new RevenuePointViewModel
            {
                Label = $"{g.Key.Month:00}/{g.Key.Year}",
                Value = g.Sum(x => x.TongCong)
            })
            .OrderBy(x => x.Label)
            .ToListAsync(cancellationToken);

        var yearlySeries = await _db.Hoadons
            .AsNoTracking()
            .Where(x => x.TrangThai == TrangThai.HoanThanh)
            .GroupBy(x => x.NgayLapHD.Year)
            .Select(g => new RevenuePointViewModel
            {
                Label = g.Key.ToString(),
                Value = g.Sum(x => x.TongCong)
            })
            .OrderBy(x => x.Label)
            .ToListAsync(cancellationToken);

        var topProducts = await _db.HoadonCTs
            .AsNoTracking()
            .Include(x => x.Hoadon)
            .Include(x => x.Sanpham)
            .Where(x => x.Hoadon.NgayLapHD >= from && x.Hoadon.NgayLapHD < to && x.Hoadon.TrangThai == TrangThai.HoanThanh)
            .GroupBy(x => new { x.IdSP, x.Sanpham.TenSP })
            .Select(g => new TopProductViewModel
            {
                ProductName = g.Key.TenSP,
                Quantity = g.Sum(x => x.SoLuong),
                Revenue = g.Sum(x => x.ThanhTien)
            })
            .OrderByDescending(x => x.Quantity)
            .Take(10)
            .ToListAsync(cancellationToken);

        return new RevenueSummaryViewModel
        {
            TotalRevenue = totalRevenue,
            InvoiceCount = invoiceCount,
            DailySeries = dailySeries,
            MonthlySeries = monthlySeries,
            YearlySeries = yearlySeries,
            TopProducts = topProducts
        };
    }

    public async Task<InventoryReportViewModel> GetInventoryReportAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var from = fromDate?.Date ?? DateTime.UtcNow.Date.AddDays(-30);
        var to = (toDate?.Date ?? DateTime.UtcNow.Date).AddDays(1);

        var nhap = await _db.Dongnhaps
            .AsNoTracking()
            .Include(x => x.Phieunhap)
            .Where(x => x.Phieunhap.NgayLapPN >= from && x.Phieunhap.NgayLapPN < to)
            .GroupBy(x => x.IdSP)
            .Select(g => new { IdSP = g.Key, Qty = g.Sum(x => x.SoLuong) })
            .ToDictionaryAsync(x => x.IdSP, x => x.Qty, cancellationToken);

        var xuat = await _db.Tieuhaos
            .AsNoTracking()
            .Include(x => x.HoadonCT)
            .ThenInclude(x => x.Hoadon)
            .Where(x => x.HoadonCT.Hoadon.NgayLapHD >= from && x.HoadonCT.Hoadon.NgayLapHD < to)
            .GroupBy(x => x.IdSP)
            .Select(g => new { IdSP = g.Key, Qty = g.Sum(x => x.SoLuong) })
            .ToDictionaryAsync(x => x.IdSP, x => x.Qty, cancellationToken);

        var products = await _db.Sanphams.AsNoTracking().OrderBy(x => x.TenSP).ToListAsync(cancellationToken);

        var rows = products.Select(p =>
        {
            nhap.TryGetValue(p.Id, out var n);
            xuat.TryGetValue(p.Id, out var x);
            return new InventoryReportRowViewModel
            {
                ProductId = p.Id,
                ProductName = p.TenSP,
                Unit = p.DonViTinh,
                Nhap = n,
                Xuat = x,
                TonCuoi = p.SoLuongTon,
                GiaTriTonKho = p.SoLuongTon * p.GiaNhap,
                SapHetHang = p.SoLuongTon <= p.SLTonMin
            };
        }).ToList();

        return new InventoryReportViewModel
        {
            FromDate = fromDate,
            ToDate = toDate,
            Rows = rows,
            TongGiaTriTonKho = rows.Sum(x => x.GiaTriTonKho),
            SoMatHangSapHet = rows.Count(x => x.SapHetHang)
        };
    }

    public async Task<byte[]> ExportInventoryExcelAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var report = await GetInventoryReportAsync(fromDate, toDate, cancellationToken);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("TonKho");
        ws.Cell(1, 1).Value = "Bao cao ton kho";
        ws.Cell(2, 1).Value = "Mat hang";
        ws.Cell(2, 2).Value = "Don vi";
        ws.Cell(2, 3).Value = "Nhap";
        ws.Cell(2, 4).Value = "Xuat";
        ws.Cell(2, 5).Value = "Ton cuoi";
        ws.Cell(2, 6).Value = "Gia tri ton";
        ws.Cell(2, 7).Value = "Canh bao";

        var row = 3;
        foreach (var r in report.Rows)
        {
            ws.Cell(row, 1).Value = r.ProductName;
            ws.Cell(row, 2).Value = r.Unit;
            ws.Cell(row, 3).Value = r.Nhap;
            ws.Cell(row, 4).Value = r.Xuat;
            ws.Cell(row, 5).Value = r.TonCuoi;
            ws.Cell(row, 6).Value = r.GiaTriTonKho;
            ws.Cell(row, 7).Value = r.SapHetHang ? "Sap het" : "OK";
            row++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportInventoryPdfAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var report = await GetInventoryReportAsync(fromDate, toDate, cancellationToken);

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Header().Text("Bao cao ton kho").FontSize(18).Bold();
                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(3);
                        c.RelativeColumn(1);
                        c.RelativeColumn(1);
                        c.RelativeColumn(1);
                        c.RelativeColumn(1);
                    });

                    table.Header(h =>
                    {
                        h.Cell().Text("Mat hang").Bold();
                        h.Cell().Text("Nhap").Bold();
                        h.Cell().Text("Xuat").Bold();
                        h.Cell().Text("Ton").Bold();
                        h.Cell().Text("Gia tri ton").Bold();
                    });

                    foreach (var r in report.Rows)
                    {
                        table.Cell().Text(r.ProductName);
                        table.Cell().Text(r.Nhap.ToString());
                        table.Cell().Text(r.Xuat.ToString());
                        table.Cell().Text(r.TonCuoi.ToString());
                        table.Cell().Text(r.GiaTriTonKho.ToString("N0"));
                    }
                });
            });
        });

        return doc.GeneratePdf();
    }

    public async Task<AdminDashboardViewModel> GetAdminDashboardAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var revenueToday = await _db.Hoadons
            .AsNoTracking()
            .Where(x => x.NgayLapHD >= today && x.NgayLapHD < tomorrow && x.TrangThai == TrangThai.HoanThanh)
            .SumAsync(x => (decimal?)x.TongCong, cancellationToken) ?? 0;

        var invoiceToday = await _db.Hoadons
            .AsNoTracking()
            .CountAsync(x => x.NgayLapHD >= today && x.NgayLapHD < tomorrow && x.TrangThai == TrangThai.HoanThanh, cancellationToken);

        var topProduct = await _db.HoadonCTs
            .AsNoTracking()
            .Include(x => x.Hoadon)
            .Include(x => x.Sanpham)
            .Where(x => x.Hoadon.NgayLapHD >= today && x.Hoadon.NgayLapHD < tomorrow && x.Hoadon.TrangThai == TrangThai.HoanThanh)
            .GroupBy(x => x.Sanpham.TenSP)
            .Select(g => new { Name = g.Key, Qty = g.Sum(x => x.SoLuong) })
            .OrderByDescending(x => x.Qty)
            .Select(x => x.Name)
            .FirstOrDefaultAsync(cancellationToken);

        var lowStock = await _db.Sanphams.AsNoTracking().CountAsync(x => x.SoLuongTon <= x.SLTonMin, cancellationToken);
        var activeEmployees = await _db.Nhanviens.AsNoTracking().CountAsync(x => x.TrangThai == TrangThaiNhanvien.DangLam, cancellationToken);

        return new AdminDashboardViewModel
        {
            RevenueToday = revenueToday,
            InvoiceToday = invoiceToday,
            TopProductToday = topProduct,
            LowStockCount = lowStock,
            ActiveEmployees = activeEmployees,
            RevenueSummary = await GetRevenueSummaryAsync(today.AddDays(-29), today, cancellationToken)
        };
    }
}
