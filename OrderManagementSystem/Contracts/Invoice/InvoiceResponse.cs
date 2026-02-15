namespace OrderManagementSystem.Contracts.Invoice;

public record InvoiceResponse(
    int InvoiceId,
    int OrderId,
    DateTime InvoiceDate,
    decimal TotalAmount
);