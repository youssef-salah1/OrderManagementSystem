using OrderManagementSystem.Abstractions;

namespace OrderManagementSystem.Errors;

public class CustomerErrors
{
    public static readonly Error NotFound = new("Customer.NotFound", "Customer with the given ID was not found.", 404);
}