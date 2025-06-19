using FruitsBasket.Data.Basket;
using Microsoft.EntityFrameworkCore;

namespace FruitsBasket.Data;

public class CosmosDbContext(DbContextOptions<CosmosDbContext> options) : DbContext(options)
{
    public DbSet<BasketDao> Baskets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BasketDao>()
            .HasQueryFilter(f => !f.IsDeleted);

        modelBuilder.HasDefaultContainer("baskets");
        modelBuilder.Entity<BasketDao>()
            .HasPartitionKey(b => b.Id);
    }
}