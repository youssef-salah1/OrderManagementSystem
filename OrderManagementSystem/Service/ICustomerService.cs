using OrderManagementSystem.Abstractions;
using OrderManagementSystem.Contracts.Customer;

namespace OrderManagementSystem.Service;

public interface ICustomerService
{
    Task<Result<CustomerResponse>> CreateCustomerAsync(CustomerCreateRequest request);
    Task<Result<CustomerResponse>> GetCustomerOrdersAsync(int id);
}