using OrderManagementSystem.Entity;

namespace OrderManagementSystem.Service;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(Customer customer);
}