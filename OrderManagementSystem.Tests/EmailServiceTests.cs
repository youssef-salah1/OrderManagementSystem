using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrderManagementSystem.Service;

namespace OrderManagementSystem.Tests;

public class EmailServiceTests
{
    private readonly Mock<ILogger<EmailService>> _loggerMock;
    private readonly EmailService _sut;

    public EmailServiceTests()
    {
        _loggerMock = new Mock<ILogger<EmailService>>();
        _sut = new EmailService(_loggerMock.Object);
    }

    [Fact]
    public async Task SendOrderStatusChangeEmailAsync_ShouldLogEmailNotification()
    {
        // Arrange
        var customerEmail = "customer@example.com";
        var orderId = 123;
        var newStatus = "Shipped";

        // Act
        await _sut.SendOrderStatusChangeEmailAsync(customerEmail, orderId, newStatus);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Email notification sent to customer@example.com")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SendOrderStatusChangeEmailAsync_ShouldCompleteSuccessfully()
    {
        // Arrange
        var customerEmail = "john.doe@example.com";
        var orderId = 456;
        var newStatus = "Delivered";

        // Act
        var action = async () => await _sut.SendOrderStatusChangeEmailAsync(customerEmail, orderId, newStatus);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SendOrderStatusChangeEmailAsync_ShouldLogCorrectOrderId()
    {
        // Arrange
        var customerEmail = "user@test.com";
        var orderId = 789;
        var newStatus = "Processing";

        // Act
        await _sut.SendOrderStatusChangeEmailAsync(customerEmail, orderId, newStatus);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Order #789")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SendOrderStatusChangeEmailAsync_ShouldLogCorrectStatus()
    {
        // Arrange
        var customerEmail = "admin@example.com";
        var orderId = 1;
        var newStatus = "Cancelled";

        // Act
        await _sut.SendOrderStatusChangeEmailAsync(customerEmail, orderId, newStatus);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Cancelled")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("customer1@test.com", 1, "Pending")]
    [InlineData("customer2@test.com", 2, "Processing")]
    [InlineData("customer3@test.com", 3, "Shipped")]
    [InlineData("customer4@test.com", 4, "Delivered")]
    [InlineData("customer5@test.com", 5, "Cancelled")]
    public async Task SendOrderStatusChangeEmailAsync_ShouldHandleMultipleScenarios(
        string email, int orderId, string status)
    {
        // Act
        await _sut.SendOrderStatusChangeEmailAsync(email, orderId, status);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => 
                    v.ToString()!.Contains(email) && 
                    v.ToString()!.Contains($"Order #{orderId}") && 
                    v.ToString()!.Contains(status)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
