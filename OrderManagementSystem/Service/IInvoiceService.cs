using OrderManagementSystem.Abstractions;
using OrderManagementSystem.Contracts.Invoice;

namespace OrderManagementSystem.Service;

public interface IInvoiceService
{
    Task<Result<InvoiceResponse>> GetInvoiceByIdAsync(int id);
    Task<Result<IEnumerable<InvoiceResponse>>> GetAllInvoicesAsync();
}