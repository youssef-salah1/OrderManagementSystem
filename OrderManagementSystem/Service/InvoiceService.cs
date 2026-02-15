using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Abstractions;
using OrderManagementSystem.Contracts.Invoice;
using OrderManagementSystem.Errors;
using OrderManagementSystem.Persistence;

namespace OrderManagementSystem.Service;

public class InvoiceService(OrderManagementDbContext context) : IInvoiceService
{
    public async Task<Result<InvoiceResponse>> GetInvoiceByIdAsync(int id)
    {
        var invoice = await context.Invoices.FindAsync(id);
        if (invoice is null) return Result.Failure<InvoiceResponse>(InvoiceErrors.NotFound);
        return Result.Success(new InvoiceResponse(invoice.InvoiceId, invoice.OrderId, invoice.InvoiceDate,
            invoice.TotalAmount));
    }

    public async Task<Result<IEnumerable<InvoiceResponse>>> GetAllInvoicesAsync()
    {
        var invoices = await context.Invoices.ToListAsync();
        var response = invoices.Select(i => new InvoiceResponse(i.InvoiceId, i.OrderId, i.InvoiceDate, i.TotalAmount));
        return Result.Success(response);
    }
}