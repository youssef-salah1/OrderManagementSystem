namespace OrderManagementSystem.Contracts;

public record UserRegisterRequest(
    string Username,
    string Password,
    string Role = "Customer"
);