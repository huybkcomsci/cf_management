using CafeManagement.Models.Entities;

namespace CafeManagement.Repositories;

public interface ISanphamRepository
{
    Task<Sanpham?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Sanpham>> GetPagedAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(string? search, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NhomSP>> GetNhomListAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Sanpham entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Sanpham entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Sanpham entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}