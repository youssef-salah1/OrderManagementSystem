using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Entity;
using OrderManagementSystem.Persistence;

namespace OrderManagementSystem.Service;

public class ProductRepository(OrderManagementDbContext context) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await context.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await context.Products.ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync();
    }
}