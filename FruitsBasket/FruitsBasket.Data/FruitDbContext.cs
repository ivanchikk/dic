using FruitsBasket.Data.Fruit;
using Microsoft.EntityFrameworkCore;

namespace FruitsBasket.Data;

public class FruitDbContext(DbContextOptions<FruitDbContext> options) : DbContext(options)
{
    public DbSet<FruitDao> Fruits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FruitDao>()
            .HasQueryFilter(f => !f.IsDeleted);
    }
}