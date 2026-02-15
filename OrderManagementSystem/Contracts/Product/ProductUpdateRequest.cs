namespace OrderManagementSystem.Contracts.Product;

public record ProductUpdateRequest(
    string Name,
    decimal Price,
    int Stock
);