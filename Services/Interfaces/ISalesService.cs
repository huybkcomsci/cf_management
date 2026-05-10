using CafeManagement.Models.ViewModels;

namespace CafeManagement.Services.Interfaces;

public interface ISalesService
{
    Task<List<SalesProductItemViewModel>> GetMenuAsync(string? keyword, CancellationToken cancellationToken = default);
    Task<SalesProductItemViewModel?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<SalesCheckoutResultViewModel> CheckoutAsync(SalesCheckoutRequestViewModel request, string? currentUserEmail, CancellationToken cancellationToken = default);
    Task<List<SalesInvoiceHistoryItemViewModel>> GetInvoiceHistoryAsync(int take = 50, CancellationToken cancellationToken = default);
    Task<List<SalesInvoiceHistoryItemViewModel>> GetInvoiceHistoryFilteredAsync(DateTime? fromDate, DateTime? toDate, int take = 50, CancellationToken cancellationToken = default);
    Task<SalesPrintViewModel?> GetPrintDataAsync(Guid hoadonId, CancellationToken cancellationToken = default);
    Task<SalesInvoiceDetailViewModel?> GetInvoiceDetailsAsync(Guid hoadonId, CancellationToken cancellationToken = default);
    Task<List<DailySalesReportViewModel>> GetDailySalesReportAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
}
