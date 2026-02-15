using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Entity;

namespace OrderManagementSystem.Persistence;

public class OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().Property(p => p.Name).HasMaxLength(100);
        modelBuilder.Entity<Customer>().Property(p => p.Email).HasMaxLength(100);

        modelBuilder.Entity<Order>().Property(p => p.PaymentMethod).HasMaxLength(100);
        modelBuilder.Entity<Order>().Property(p => p.Status).HasMaxLength(50);

        modelBuilder.Entity<Product>().Property(p => p.Name).HasMaxLength(500);

        modelBuilder.Entity<User>().Property(p => p.Role).HasMaxLength(100);

        base.OnModelCreating(modelBuilder);
    }
}