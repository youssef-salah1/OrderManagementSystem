using OrderManagementSystem.Contracts.Order;

namespace OrderManagementSystem.Contracts.Customer;

public record CustomerResponse(
    int CustomerId,
    string Name,
    string Email,
    List<OrderResponse> Orders
);