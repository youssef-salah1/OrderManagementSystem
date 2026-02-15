namespace OrderManagementSystem.Contracts.Order;

public record OrderItemRequest(
    int ProductId,
    int Quantity
);