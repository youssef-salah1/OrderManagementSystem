using OrderManagementSystem.Entity;

namespace OrderManagementSystem.Authentication;

public interface IJwtProvider
{
    string Generate(User user);
}