namespace OrderManagementSystem.Contracts.Product;

public record ProductCreateRequest(
    string Name,
    decimal Price,
    int Stock
);