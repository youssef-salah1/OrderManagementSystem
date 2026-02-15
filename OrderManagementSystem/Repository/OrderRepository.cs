using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Entity;
using OrderManagementSystem.Persistence;

namespace OrderManagementSystem.Service;

public class OrderRepository(OrderManagementDbContext context) : IOrderRepository
{
    private readonly OrderManagementDbContext _context = context;

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Include(o => o.Invoice)
            .FirstOrDefaultAsync(o => o.OrderId == id);
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Include(o => o.Invoice)
            .ToListAsync();
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(int orderId, string status)
    {
        await context.Orders
            .Where(o => o.OrderId == orderId)
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.Status, status));
    }
}