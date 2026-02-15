using FluentValidation;
using OrderManagementSystem.Contracts.Order;

namespace OrderManagementSystem.Validation;

public class OrderCreateRequestValidator : AbstractValidator<OrderCreateRequest>
{
    public OrderCreateRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("A valid Customer ID is required.");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("Payment Method is required.")
            .Must(method => new[] { "Credit Card", "PayPal", "Cash" }.Contains(method))
            .WithMessage("Payment Method must be 'Credit Card', 'PayPal', or 'Cash'.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must contain at least one item.");

        // Validates each item in the list using the child validator below
        RuleForEach(x => x.Items).SetValidator(new OrderItemRequestValidator());
    }
}