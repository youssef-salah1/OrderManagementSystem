using OrderManagementSystem.Abstractions;
using OrderManagementSystem.Contracts.Order;

namespace OrderManagementSystem.Service;

public interface IOrderService
{
    Task<Result<OrderResponse>> CreateOrderAsync(OrderCreateRequest request);
    Task<Result<OrderResponse>> GetOrderByIdAsync(int orderId);
    Task<Result<IEnumerable<OrderResponse>>> GetAllOrdersAsync();
    Task<Result> UpdateOrderStatusAsync(int orderId, string status);
}