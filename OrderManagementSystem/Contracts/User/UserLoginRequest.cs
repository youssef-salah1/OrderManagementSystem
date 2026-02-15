namespace OrderManagementSystem.Contracts;

public record UserLoginRequest(
    string Username,
    string Password
);