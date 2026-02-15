using FluentAssertions;
using Moq;
using OrderManagementSystem.Contracts.Customer;
using OrderManagementSystem.Entity;
using OrderManagementSystem.Service;

namespace OrderManagementSystem.Tests;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly CustomerService _sut;

    public CustomerServiceTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _sut = new CustomerService(_customerRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateCustomerAsync_WhenValidRequest_ShouldCreateCustomer()
    {
        // Arrange
        var request = new CustomerCreateRequest("John Doe", "john.doe@example.com");

        _customerRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Customer>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateCustomerAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("John Doe");
        result.Value.Email.Should().Be("john.doe@example.com");
        result.Value.Orders.Should().BeEmpty();
        _customerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task CreateCustomerAsync_ShouldSetCustomerProperties()
    {
        // Arrange
        var request = new CustomerCreateRequest("Jane Smith", "jane.smith@example.com");

        Customer? capturedCustomer = null;
        _customerRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Customer>()))
            .Callback<Customer>(customer => capturedCustomer = customer)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateCustomerAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedCustomer.Should().NotBeNull();
        capturedCustomer!.Name.Should().Be("Jane Smith");
        capturedCustomer.Email.Should().Be("jane.smith@example.com");
    }

    [Fact]
    public async Task GetCustomerOrdersAsync_WhenCustomerDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _sut.GetCustomerOrdersAsync(999);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Customer.NotFound");
    }

    [Fact]
    public async Task GetCustomerOrdersAsync_WhenCustomerExists_ShouldReturnCustomerWithOrders()
    {
        // Arrange
        var customer = new Customer
        {
            CustomerId = 1,
            Name = "John Doe",
            Email = "john.doe@example.com",
            Orders = new List<Order>
            {
                new Order
                {
                    OrderId = 1,
                    CustomerId = 1,
                    TotalAmount = 100m,
                    PaymentMethod = "Credit Card",
                    Status = "Pending",
                    OrderItems = new List<OrderItem>()
                },
                new Order
                {
                    OrderId = 2,
                    CustomerId = 1,
                    TotalAmount = 200m,
                    PaymentMethod = "PayPal",
                    Status = "Shipped",
                    OrderItems = new List<OrderItem>()
                }
            }
        };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(customer);

        // Act
        var result = await _sut.GetCustomerOrdersAsync(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CustomerId.Should().Be(1);
        result.Value.Name.Should().Be("John Doe");
        result.Value.Email.Should().Be("john.doe@example.com");
        result.Value.Orders.Should().HaveCount(2);
        result.Value.Orders.First().OrderId.Should().Be(1);
        result.Value.Orders.Last().OrderId.Should().Be(2);
    }

    [Fact]
    public async Task GetCustomerOrdersAsync_WhenCustomerHasNoOrders_ShouldReturnEmptyOrdersList()
    {
        // Arrange
        var customer = new Customer
        {
            CustomerId = 1,
            Name = "John Doe",
            Email = "john.doe@example.com",
            Orders = new List<Order>()
        };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(customer);

        // Act
        var result = await _sut.GetCustomerOrdersAsync(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Orders.Should().BeEmpty();
    }
}
