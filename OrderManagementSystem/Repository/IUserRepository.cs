using OrderManagementSystem.Entity;

namespace OrderManagementSystem.Service;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
}