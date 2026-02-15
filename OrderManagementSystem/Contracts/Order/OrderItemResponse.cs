namespace OrderManagementSystem.Contracts.Order;

public record OrderItemResponse(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Discount
);