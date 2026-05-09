using Microsoft.EntityFrameworkCore;

public class CafeDbContext : DbContext
{
    public CafeDbContext(DbContextOptions<CafeDbContext> options) : base(options)
    {
    }

    // Add your DbSets here for your entities
    // Example:
    // public DbSet<Product> Products { get; set; }
    // public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Add your entity configurations here
    }
}
