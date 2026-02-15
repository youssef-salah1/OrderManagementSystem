namespace OrderManagementSystem.Contracts.Product;

public record ProductResponse(
    int ProductId,
    string Name,
    decimal Price,
    int Stock
);