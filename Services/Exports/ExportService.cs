using CafeManagement.Data;
using CafeManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Services.Exports;

public class ExportService : IExportService
{
    private readonly ApplicationDbContext _db;
    private readonly IReportService _reportService;
    private readonly ISalesService _salesService;
    private readonly PdfTemplateService _pdfTemplateService;
    private readonly ExcelTemplateService _excelTemplateService;
    private readonly ILogger<ExportService> _logger;

    public ExportService(
        ApplicationDbContext db,
        IReportService reportService,
        ISalesService salesService,
        PdfTemplateService pdfTemplateService,
        ExcelTemplateService excelTemplateService,
        ILogger<ExportService> logger)
    {
        _db = db;
        _reportService = reportService;
        _salesService = salesService;
        _pdfTemplateService = pdfTemplateService;
        _excelTemplateService = excelTemplateService;
        _logger = logger;
    }

    #region Invoice Exports

    public async Task<byte[]> ExportInvoiceToPdfAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var invoice = await _db.Hoadons
                .AsNoTracking()
                .Include(x => x.HoadonCTs)
                    .ThenInclude(x => x.Sanpham)
                .FirstOrDefaultAsync(x => x.Id == invoiceId, cancellationToken);

            if (invoice == null)
                throw new InvalidOperationException("Không tìm thấy hóa đơn");

            var items = invoice.HoadonCTs
                .Select(x => (
                    productName: x.Sanpham?.TenSP ?? "Sản phẩm",
                    quantity: x.SoLuong,
                    unitPrice: x.DonGia,
                    total: x.ThanhTien
                ))
                .ToList();

            var pdf = _pdfTemplateService.GenerateInvoicePdf(
                $"HD-{invoice.Id.ToString().Substring(0, 8)}",
                invoice.NgayLapHD,
                "Khách lẻ",
                items,
                invoice.ThanhTien,
                invoice.ThanhTien * 0.1m,
                invoice.TongCong,
                invoice.GhiChu ?? ""
            );

            return pdf;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting invoice to PDF");
            throw;
        }
    }

    public async Task<byte[]> ExportInvoiceToExcelAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var invoice = await _db.Hoadons
                .AsNoTracking()
                .Include(x => x.HoadonCTs)
                    .ThenInclude(x => x.Sanpham)
                .FirstOrDefaultAsync(x => x.Id == invoiceId, cancellationToken);

            if (invoice == null)
                throw new InvalidOperationException("Không tìm thấy hóa đơn");

            var items = invoice.HoadonCTs
                .Select(x => (
                    productName: x.Sanpham?.TenSP ?? "Sản phẩm",
                    quantity: x.SoLuong,
                    unitPrice: x.DonGia,
                    total: x.ThanhTien
                ))
                .ToList();

            var excel = _excelTemplateService.GenerateInvoiceExcel(
                $"HD-{invoice.Id.ToString().Substring(0, 8)}",
                invoice.NgayLapHD,
                "Khách lẻ",
                items,
                invoice.ThanhTien,
                invoice.ThanhTien * 0.1m,
                invoice.TongCong,
                invoice.GhiChu ?? ""
            );

            return excel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting invoice to Excel");
            throw;
        }
    }

    #endregion

    #region Revenue Report Exports

    public async Task<byte[]> ExportRevenueReportToPdfAsync(
        DateTime fromDate,
        DateTime toDate,
        string reportType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var data = await BuildRevenueData(fromDate, toDate, reportType, cancellationToken);

            var pdf = _pdfTemplateService.GenerateRevenueReportPdf(
                fromDate,
                toDate,
                reportType,
                data.Items,
                data.Total
            );

            return pdf;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting revenue report to PDF");
            throw;
        }
    }

    public async Task<byte[]> ExportRevenueReportToExcelAsync(
        DateTime fromDate,
        DateTime toDate,
        string reportType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var data = await BuildRevenueData(fromDate, toDate, reportType, cancellationToken);

            var excel = _excelTemplateService.GenerateRevenueReportExcel(
                fromDate,
                toDate,
                reportType,
                data.Items,
                data.Total
            );

            return excel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting revenue report to Excel");
            throw;
        }
    }

    private async Task<(List<(string, decimal)> Items, decimal Total)> BuildRevenueData(
        DateTime fromDate,
        DateTime toDate,
        string reportType,
        CancellationToken cancellationToken)
    {
        var items = new List<(string, decimal)>();
        decimal total = 0;

        if (reportType == "daily")
        {
            var current = fromDate.Date;
            while (current <= toDate.Date)
            {
                var dayRevenue = await _db.Hoadons
                    .AsNoTracking()
                    .Where(x => x.NgayLapHD.Date == current)
                    .SumAsync(x => x.TongCong, cancellationToken);

                items.Add(($"Ngày {current:dd/MM/yyyy}", dayRevenue));
                total += dayRevenue;
                current = current.AddDays(1);
            }
        }
        else if (reportType == "monthly")
        {
            var current = new DateTime(fromDate.Year, fromDate.Month, 1);
            while (current <= toDate)
            {
                var nextMonth = current.AddMonths(1);
                var monthRevenue = await _db.Hoadons
                    .AsNoTracking()
                    .Where(x => x.NgayLapHD >= current && x.NgayLapHD < nextMonth)
                    .SumAsync(x => x.TongCong, cancellationToken);

                items.Add(($"Tháng {current:MM/yyyy}", monthRevenue));
                total += monthRevenue;
                current = nextMonth;
            }
        }
        else
        {
            var fromYear = fromDate.Year;
            var toYear = toDate.Year;
            for (int year = fromYear; year <= toYear; year++)
            {
                var yearRevenue = await _db.Hoadons
                    .AsNoTracking()
                    .Where(x => x.NgayLapHD.Year == year)
                    .SumAsync(x => x.TongCong, cancellationToken);

                items.Add(($"Năm {year}", yearRevenue));
                total += yearRevenue;
            }
        }

        return (items, total);
    }

    #endregion

    #region Inventory Report Exports

    public async Task<byte[]> ExportInventoryReportToPdfAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var inventory = await BuildInventoryData(cancellationToken);

            var pdf = _pdfTemplateService.GenerateInventoryReportPdf(inventory);
            return pdf;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting inventory report to PDF");
            throw;
        }
    }

    public async Task<byte[]> ExportInventoryReportToExcelAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var inventory = await BuildInventoryData(cancellationToken);

            var excel = _excelTemplateService.GenerateInventoryReportExcel(inventory);
            return excel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting inventory report to Excel");
            throw;
        }
    }

    private async Task<List<(string, string, int, int, decimal, decimal, string)>> BuildInventoryData(CancellationToken cancellationToken)
    {
        var products = await _db.Sanphams
            .AsNoTracking()
            .OrderBy(x => x.TenSP)
            .ToListAsync(cancellationToken);

        var inventory = new List<(string, string, int, int, decimal, decimal, string)>();

        foreach (var product in products)
        {
            var unitCost = product.GiaNhap;
            var totalValue = product.SoLuongTon * unitCost;
            var status = product.SoLuongTon < product.SLTonMin ? "Cảnh báo: Sắp hết" : "Bình thường";

            inventory.Add((
                product.TenSP,
                product.DonViTinh ?? "cái",
                product.SoLuongTon,
                product.SLTonMin,
                unitCost,
                totalValue,
                status
            ));
        }

        return inventory;
    }

    #endregion
}
