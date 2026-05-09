using CafeManagement.DTOs;

namespace CafeManagement.Services.Interfaces;

public interface ISanphamService
{
    Task<SanphamIndexVm> GetPagedAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<SanphamDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NhomOptionDto>> GetNhomOptionsAsync(CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(SanphamUpsertDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, SanphamUpsertDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}