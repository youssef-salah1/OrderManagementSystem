namespace OrderManagementSystem.Service;

public interface IEmailService
{
    Task SendOrderStatusChangeEmailAsync(string customerEmail, int orderId, string newStatus);
}
