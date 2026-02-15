using OrderManagementSystem.Abstractions;

namespace OrderManagementSystem.Errors;

public static class InvoiceErrors
{
    public static readonly Error NotFound = new("Invoice.NotFound", "Invoice not found.", 404);
}