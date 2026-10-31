using RestaurantApi.Models;
using Microsoft.EntityFrameworkCore;

namespace RestaurantApi.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options) {}

    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.ToTable("Menus");
            entity.Property(m => m.Name).IsRequired();
            entity.Property(m => m.Price).HasPrecision(18, 2);
        });

        // One Menu has many Orders.
        modelBuilder.Entity<Order>()
          .HasOne(o => o.Menu)
          .WithMany(m => m.Orders)
          .HasForeignKey(o => o.MenuId)
          .OnDelete(DeleteBehavior.Restrict);
    }
}