namespace OrderManagementSystem.Contracts.Customer;

public record CustomerCreateRequest(
    string Name,
    string Email
);