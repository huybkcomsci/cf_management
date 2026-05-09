using CafeManagement.Models.Entities;

namespace CafeManagement.Repositories;

public interface IProductRepository : IRepository<Product>
{
    // Add product-specific repository methods here
    Task<IEnumerable<Product>> GetTopAsync(int count);
}
