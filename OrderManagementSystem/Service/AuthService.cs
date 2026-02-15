using Microsoft.AspNetCore.Identity;
using OrderManagementSystem.Abstractions;
using OrderManagementSystem.Authentication;
using OrderManagementSystem.Contracts;
using OrderManagementSystem.Entity;

namespace OrderManagementSystem.Service;

public class AuthService(
    IUserRepository userRepository,
    IJwtProvider jwtProvider
) : IAuthService
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public async Task<Result<UserRegisterResponse>> RegisterAsync(UserRegisterRequest request)
    {
        var existingUser = await userRepository.GetByUsernameAsync(request.Username);
        if (existingUser is not null)
            return Result.Failure<UserRegisterResponse>(new Error("User.Exists", "Username already exists", 409));

        var user = new User
        {
            Username = request.Username,
            Role = request.Role
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await userRepository.AddAsync(user);

        return Result.Success(new UserRegisterResponse(
            user.UserId,
            user.Username
        ));
    }

    public async Task<Result<UserLoginResponse>> LoginAsync(UserLoginRequest request)
    {
        var user = await userRepository.GetByUsernameAsync(request.Username);
        if (user is null)
            return Result.Failure<UserLoginResponse>(new Error("Auth.Invalid", "Invalid username or password", 400));

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
            return Result.Failure<UserLoginResponse>(new Error("Auth.Invalid", "Invalid username or password", 400));

        var token = jwtProvider.Generate(user);

        return Result.Success(new UserLoginResponse(
            token,
            user.Role,
            user.UserId
        ));
    }
}