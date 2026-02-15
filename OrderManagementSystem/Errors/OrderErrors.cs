using OrderManagementSystem.Abstractions;

namespace OrderManagementSystem.Errors;

public class OrderErrors
{
    public static readonly Error NotFound = new("Order.NotFound", "The specified order was not found.", 404);

    public static readonly Error InsufficientStock =
        new("Order.InsufficientStock", "One or more products have insufficient stock.", 400);

    public static readonly Error InvalidItems = new("Order.InvalidItems", "Order must contain at least one item.", 400);
}