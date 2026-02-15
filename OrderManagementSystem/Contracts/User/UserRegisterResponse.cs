namespace OrderManagementSystem.Contracts;

public record UserRegisterResponse(
    int UserId,
    string Username
);