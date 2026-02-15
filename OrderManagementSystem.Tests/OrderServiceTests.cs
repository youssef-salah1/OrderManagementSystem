using FluentAssertions;
using Moq;
using OrderManagementSystem.Contracts.Order;
using OrderManagementSystem.Entity;
using OrderManagementSystem.Service;

namespace OrderManagementSystem.Tests;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _sut = new OrderService(
            _orderRepositoryMock.Object,
            _productRepositoryMock.Object,
            _customerRepositoryMock.Object,
            _emailServiceMock.Object
        );
    }

    [Fact]
    public async Task CreateOrderAsync_WhenCustomerDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var request = new OrderCreateRequest(
            CustomerId: 999,
            PaymentMethod: "Credit Card",
            Items: new List<OrderItemRequest>()
        );

        _customerRepositoryMock
            .Setup(x => x.ExistsAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.CreateOrderAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Customer.NotFound");
    }

    [Fact]
    public async Task CreateOrderAsync_WhenProductDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var request = new OrderCreateRequest(
            CustomerId: 1,
            PaymentMethod: "Credit Card",
            Items: new List<OrderItemRequest>
            {
                new(ProductId: 999, Quantity: 1)
            }
        );

        _customerRepositoryMock
            .Setup(x => x.ExistsAsync(1))
            .ReturnsAsync(true);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.CreateOrderAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.NotFound");
    }

    [Fact]
    public async Task CreateOrderAsync_WhenInsufficientStock_ShouldReturnFailure()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            Name = "Test Product",
            Price = 50m,
            Stock = 5
        };

        var request = new OrderCreateRequest(
            CustomerId: 1,
            PaymentMethod: "Credit Card",
            Items: new List<OrderItemRequest>
            {
                new(ProductId: 1, Quantity: 10) // Requesting more than available
            }
        );

        _customerRepositoryMock
            .Setup(x => x.ExistsAsync(1))
            .ReturnsAsync(true);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _sut.CreateOrderAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Order.InsufficientStock");
    }

    [Fact]
    public async Task CreateOrderAsync_WhenOrderTotalIsLessThan100_ShouldNotApplyDiscount()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            Name = "Test Product",
            Price = 40m,
            Stock = 10
        };

        var request = new OrderCreateRequest(
            CustomerId: 1,
            PaymentMethod: "Credit Card",
            Items: new List<OrderItemRequest>
            {
                new(ProductId: 1, Quantity: 2) // Total: $80
            }
        );

        _customerRepositoryMock
            .Setup(x => x.ExistsAsync(1))
            .ReturnsAsync(true);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        _orderRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalAmount.Should().Be(80m); // No discount
    }

    [Fact]
    public async Task CreateOrderAsync_WhenOrderTotalIsBetween100And200_ShouldApply5PercentDiscount()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            Name = "Test Product",
            Price = 75m,
            Stock = 10
        };

        var request = new OrderCreateRequest(
            CustomerId: 1,
            PaymentMethod: "Credit Card",
            Items: new List<OrderItemRequest>
            {
                new(ProductId: 1, Quantity: 2) // Total: $150, After 5% discount: $142.50
            }
        );

        _customerRepositoryMock
            .Setup(x => x.ExistsAsync(1))
            .ReturnsAsync(true);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        _orderRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalAmount.Should().Be(142.50m); // 5% discount applied
    }

    [Fact]
    public async Task CreateOrderAsync_WhenOrderTotalIsOver200_ShouldApply10PercentDiscount()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            Name = "Test Product",
            Price = 150m,
            Stock = 10
        };

        var request = new OrderCreateRequest(
            CustomerId: 1,
            PaymentMethod: "Credit Card",
            Items: new List<OrderItemRequest>
            {
                new(ProductId: 1, Quantity: 2) // Total: $300, After 10% discount: $270
            }
        );

        _customerRepositoryMock
            .Setup(x => x.ExistsAsync(1))
            .ReturnsAsync(true);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        _orderRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalAmount.Should().Be(270m); // 10% discount applied
    }

    [Fact]
    public async Task CreateOrderAsync_WhenSuccessful_ShouldUpdateProductStock()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            Name = "Test Product",
            Price = 50m,
            Stock = 10
        };

        var request = new OrderCreateRequest(
            CustomerId: 1,
            PaymentMethod: "Credit Card",
            Items: new List<OrderItemRequest>
            {
                new(ProductId: 1, Quantity: 3)
            }
        );

        _customerRepositoryMock
            .Setup(x => x.ExistsAsync(1))
            .ReturnsAsync(true);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        _orderRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Stock.Should().Be(7); // 10 - 3
        _productRepositoryMock.Verify(x => x.UpdateAsync(product), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenSuccessful_ShouldCreateInvoice()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            Name = "Test Product",
            Price = 100m,
            Stock = 10
        };

        var request = new OrderCreateRequest(
            CustomerId: 1,
            PaymentMethod: "PayPal",
            Items: new List<OrderItemRequest>
            {
                new(ProductId: 1, Quantity: 1)
            }
        );

        _customerRepositoryMock
            .Setup(x => x.ExistsAsync(1))
            .ReturnsAsync(true);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        Order? capturedOrder = null;
        _orderRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Order>()))
            .Callback<Order>(order => capturedOrder = order)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedOrder.Should().NotBeNull();
        capturedOrder!.Invoice.Should().NotBeNull();
        capturedOrder.Invoice.TotalAmount.Should().Be(capturedOrder.TotalAmount);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WhenOrderDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        _orderRepositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _sut.UpdateOrderStatusAsync(999, "Shipped");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Order.NotFound");
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WhenSuccessful_ShouldSendEmailNotification()
    {
        // Arrange
        var order = new Order
        {
            OrderId = 1,
            CustomerId = 1,
            TotalAmount = 100m,
            PaymentMethod = "Credit Card",
            Status = "Pending"
        };

        var customer = new Customer
        {
            CustomerId = 1,
            Name = "John Doe",
            Email = "john.doe@example.com"
        };

        _orderRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(order);

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(customer);

        _orderRepositoryMock
            .Setup(x => x.UpdateStatusAsync(1, "Shipped"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateOrderStatusAsync(1, "Shipped");

        // Assert
        result.IsSuccess.Should().BeTrue();
        _emailServiceMock.Verify(
            x => x.SendOrderStatusChangeEmailAsync("john.doe@example.com", 1, "Shipped"),
            Times.Once
        );
    }

    [Fact]
    public async Task GetOrderByIdAsync_WhenOrderDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        _orderRepositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _sut.GetOrderByIdAsync(999);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Order.NotFound");
    }

    [Fact]
    public async Task GetOrderByIdAsync_WhenOrderExists_ShouldReturnOrder()
    {
        // Arrange
        var order = new Order
        {
            OrderId = 1,
            CustomerId = 1,
            TotalAmount = 100m,
            PaymentMethod = "Credit Card",
            Status = "Pending",
            OrderItems = new List<OrderItem>()
        };

        _orderRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(order);

        // Act
        var result = await _sut.GetOrderByIdAsync(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.OrderId.Should().Be(1);
        result.Value.TotalAmount.Should().Be(100m);
    }
}
