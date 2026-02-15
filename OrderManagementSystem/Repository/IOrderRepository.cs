using OrderManagementSystem.Entity;

namespace OrderManagementSystem.Service;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<IEnumerable<Order>> GetAllAsync();
    Task AddAsync(Order order);
    Task UpdateStatusAsync(int orderId, string status);
}