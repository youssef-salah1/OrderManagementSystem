using OrderManagementSystem.Abstractions;
using OrderManagementSystem.Contracts.Customer;
using OrderManagementSystem.Contracts.Order;
using OrderManagementSystem.Entity;
using OrderManagementSystem.Errors;

namespace OrderManagementSystem.Service;

public class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    public async Task<Result<CustomerResponse>> CreateCustomerAsync(CustomerCreateRequest request)
    {
        var customer = new Customer { Name = request.Name, Email = request.Email };
        await customerRepository.AddAsync(customer);
        return Result.Success(new CustomerResponse(customer.CustomerId, customer.Name, customer.Email,
            new List<OrderResponse>()));
    }

    public async Task<Result<CustomerResponse>> GetCustomerOrdersAsync(int id)
    {
        var customer = await customerRepository.GetByIdAsync(id);
        if (customer is null) return Result.Failure<CustomerResponse>(CustomerErrors.NotFound);

        // Map Orders
        var orderResponses = customer.Orders.Select(o => new OrderResponse(
            o.OrderId, o.CustomerId, o.OrderDate, o.TotalAmount, o.PaymentMethod, o.Status,
            new List<OrderItemResponse>() // Simplified
        )).ToList();

        return Result.Success(new CustomerResponse(customer.CustomerId, customer.Name, customer.Email, orderResponses));
    }
}