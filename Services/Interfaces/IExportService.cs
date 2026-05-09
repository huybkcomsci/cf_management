namespace CafeManagement.Services.Interfaces;

public interface IExportService
{
    // Invoice exports
    Task<byte[]> ExportInvoiceToPdfAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<byte[]> ExportInvoiceToExcelAsync(Guid invoiceId, CancellationToken cancellationToken = default);

    // Revenue report exports
    Task<byte[]> ExportRevenueReportToPdfAsync(DateTime fromDate, DateTime toDate, string reportType, CancellationToken cancellationToken = default);
    Task<byte[]> ExportRevenueReportToExcelAsync(DateTime fromDate, DateTime toDate, string reportType, CancellationToken cancellationToken = default);

    // Inventory report exports
    Task<byte[]> ExportInventoryReportToPdfAsync(CancellationToken cancellationToken = default);
    Task<byte[]> ExportInventoryReportToExcelAsync(CancellationToken cancellationToken = default);
}
