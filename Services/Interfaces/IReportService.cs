using CafeManagement.Models.ViewModels;

namespace CafeManagement.Services.Interfaces;

public interface IReportService
{
    Task<RevenueSummaryViewModel> GetRevenueSummaryAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
    Task<InventoryReportViewModel> GetInventoryReportAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
    Task<byte[]> ExportInventoryExcelAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
    Task<byte[]> ExportInventoryPdfAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
    Task<AdminDashboardViewModel> GetAdminDashboardAsync(CancellationToken cancellationToken = default);
}
