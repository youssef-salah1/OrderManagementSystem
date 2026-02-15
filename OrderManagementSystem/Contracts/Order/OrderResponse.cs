namespace OrderManagementSystem.Contracts.Order;

public record OrderResponse(
    int OrderId,
    int CustomerId,
    DateTime OrderDate,
    decimal TotalAmount,
    string PaymentMethod,
    string Status,
    List<OrderItemResponse> Items
);