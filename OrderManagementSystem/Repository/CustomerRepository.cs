using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Entity;
using OrderManagementSystem.Persistence;

namespace OrderManagementSystem.Service;

public class CustomerRepository(OrderManagementDbContext context) : ICustomerRepository
{
    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await context.Customers
            .Include(c => c.Orders) // Include orders for history
            .FirstOrDefaultAsync(c => c.CustomerId == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Customers.AnyAsync(c => c.CustomerId == id);
    }

    public async Task AddAsync(Customer customer)
    {
        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();
    }
}