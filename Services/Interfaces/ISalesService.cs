using CafeManagement.Models.ViewModels;

namespace CafeManagement.Services.Interfaces;

public interface ISalesService
{
    Task<List<SalesProductItemViewModel>> GetMenuAsync(string? keyword, CancellationToken cancellationToken = default);
    Task<SalesProductItemViewModel?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<SalesCheckoutResultViewModel> CheckoutAsync(SalesCheckoutRequestViewModel request, string? currentUserEmail, CancellationToken cancellationToken = default);
    Task<SalesPrintViewModel?> GetPrintDataAsync(Guid hoadonId, CancellationToken cancellationToken = default);
}
