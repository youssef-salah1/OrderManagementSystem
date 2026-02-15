namespace OrderManagementSystem.Contracts;

public record UserLoginResponse(
    string Token,
    string Role,
    int UserId
);