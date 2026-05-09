using CafeManagement.Data;
using CafeManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Repositories.Implementations;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetTopAsync(int count)
    {
        return await _dbSet.OrderByDescending(p => p.Id).Take(count).ToListAsync();
    }
}
