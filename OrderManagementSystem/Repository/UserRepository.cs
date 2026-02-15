using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Entity;
using OrderManagementSystem.Persistence;

namespace OrderManagementSystem.Service;

public class UserRepository(OrderManagementDbContext context) : IUserRepository
{
    private readonly OrderManagementDbContext _context = context;

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .SingleOrDefaultAsync(u => u.Username == username);
    }

    public async Task AddAsync(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }
}