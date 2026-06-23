using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Persistence;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<ProductReadModel> Products => Set<ProductReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(b =>
        {
            b.HasKey(o => o.Id);
            b.OwnsMany(o => o.Items, ib =>
            {
                ib.HasKey(i => i.Id);
                ib.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
            });
            b.Ignore(o => o.DomainEvents);
        });

        modelBuilder.Entity<ProductReadModel>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Price).HasColumnType("decimal(18,2)");
        });
    }
}