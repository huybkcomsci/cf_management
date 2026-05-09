using CafeManagement.Data;
using CafeManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Repositories.Implementations;

public class SanphamRepository : ISanphamRepository
{
    private readonly ApplicationDbContext _context;

    public SanphamRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Sanpham?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sanphams
            .Include(x => x.NhomSP)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Sanpham>> GetPagedAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Sanphams.Include(x => x.NhomSP).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.TenSP.Contains(search) || (x.MoTa != null && x.MoTa.Contains(search)));
        }

        return await query
            .OrderByDescending(x => x.NgayTao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(string? search, CancellationToken cancellationToken = default)
    {
        var query = _context.Sanphams.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.TenSP.Contains(search) || (x.MoTa != null && x.MoTa.Contains(search)));
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NhomSP>> GetNhomListAsync(CancellationToken cancellationToken = default)
    {
        return await _context.NhomSPs.OrderBy(x => x.TenNhom).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Sanpham entity, CancellationToken cancellationToken = default)
    {
        await _context.Sanphams.AddAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(Sanpham entity, CancellationToken cancellationToken = default)
    {
        _context.Sanphams.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Sanpham entity, CancellationToken cancellationToken = default)
    {
        _context.Sanphams.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}