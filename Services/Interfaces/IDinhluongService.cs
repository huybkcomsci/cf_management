using CafeManagement.Models.ViewModels;

namespace CafeManagement.Services.Interfaces;

public interface IDinhluongService
{
    Task<IReadOnlyList<DinhluongListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DinhluongEditViewModel> BuildCreateModelAsync(CancellationToken cancellationToken = default);
    Task<DinhluongEditViewModel?> BuildEditModelAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(DinhluongEditViewModel model, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, DinhluongEditViewModel model, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DinhluongListItemViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
