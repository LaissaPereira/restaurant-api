using RestaurantApi.Models;
using Microsoft.EntityFrameworkCore;

namespace RestaurantApi.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options) {}

    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<Order> Orders => Set<Order>();

    public DbSet<PreReceipt> PreReceipts => Set<PreReceipt>();

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

        // One Order has at most one PreReceipt; PreReceipt is the dependent side (holds OrderId).
        modelBuilder.Entity<PreReceipt>(entity =>
        {
            entity.ToTable("PreReceipts");
            entity.HasOne(pr => pr.Order)
              .WithOne(o => o.PreReceipt)
              .HasForeignKey<PreReceipt>(pr => pr.OrderId)
              .OnDelete(DeleteBehavior.Cascade);
        });
    }
}