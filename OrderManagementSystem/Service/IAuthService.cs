using OrderManagementSystem.Abstractions;
using OrderManagementSystem.Contracts;

namespace OrderManagementSystem.Service;

public interface IAuthService
{
    Task<Result<UserRegisterResponse>> RegisterAsync(UserRegisterRequest request);
    Task<Result<UserLoginResponse>> LoginAsync(UserLoginRequest request);
}