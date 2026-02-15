namespace OrderManagementSystem.Contracts.Order;

public record OrderCreateRequest(
    int CustomerId,
    string PaymentMethod,
    List<OrderItemRequest> Items
);