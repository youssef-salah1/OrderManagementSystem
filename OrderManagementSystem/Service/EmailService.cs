namespace OrderManagementSystem.Service;

public class EmailService(ILogger<EmailService> logger) : IEmailService
{
    private readonly ILogger<EmailService> _logger = logger;

    public async Task SendOrderStatusChangeEmailAsync(string customerEmail, int orderId, string newStatus)
    {
        // In a real application, this would integrate with an email service like SendGrid, AWS SES, etc.
        // For this implementation, we'll just log the email that would be sent
        
        await Task.CompletedTask;
        
        _logger.LogInformation(
            "Email notification sent to {Email} - Order #{OrderId} status changed to {Status}",
            customerEmail, 
            orderId, 
            newStatus);
        
        // Example implementation with a real email service:
        // var message = new EmailMessage
        // {
        //     To = customerEmail,
        //     Subject = $"Order #{orderId} Status Update",
        //     Body = $"Your order #{orderId} status has been changed to {newStatus}."
        // };
        // await _emailClient.SendAsync(message);
    }
}
