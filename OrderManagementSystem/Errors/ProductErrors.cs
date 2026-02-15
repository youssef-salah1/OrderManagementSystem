using OrderManagementSystem.Abstractions;

namespace OrderManagementSystem.Errors;

public class ProductErrors
{
    public static readonly Error NotFound = new("Product.NotFound", "Product with the given ID was not found.", 404);
}