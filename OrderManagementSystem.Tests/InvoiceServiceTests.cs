using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Entity;
using OrderManagementSystem.Persistence;
using OrderManagementSystem.Service;

namespace OrderManagementSystem.Tests;

public class InvoiceServiceTests : IDisposable
{
    private readonly OrderManagementDbContext _context;
    private readonly InvoiceService _sut;

    public InvoiceServiceTests()
    {
        var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new OrderManagementDbContext(options);
        _sut = new InvoiceService(_context);
    }

    [Fact]
    public async Task GetInvoiceByIdAsync_WhenInvoiceExists_ShouldReturnInvoice()
    {
        // Arrange
        var invoice = new Invoice
        {
            InvoiceId = 1,
            OrderId = 1,
            InvoiceDate = DateTime.UtcNow,
            TotalAmount = 500m
        };

        await _context.Invoices.AddAsync(invoice);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetInvoiceByIdAsync(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.InvoiceId.Should().Be(1);
        result.Value.OrderId.Should().Be(1);
        result.Value.TotalAmount.Should().Be(500m);
    }

    [Fact]
    public async Task GetInvoiceByIdAsync_WhenInvoiceDoesNotExist_ShouldReturnFailure()
    {
        // Act
        var result = await _sut.GetInvoiceByIdAsync(999);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Invoice.NotFound");
    }

    [Fact]
    public async Task GetAllInvoicesAsync_ShouldReturnAllInvoices()
    {
        // Arrange
        var invoices = new List<Invoice>
        {
            new Invoice { InvoiceId = 1, OrderId = 1, InvoiceDate = DateTime.UtcNow, TotalAmount = 100m },
            new Invoice { InvoiceId = 2, OrderId = 2, InvoiceDate = DateTime.UtcNow, TotalAmount = 200m },
            new Invoice { InvoiceId = 3, OrderId = 3, InvoiceDate = DateTime.UtcNow, TotalAmount = 300m }
        };

        await _context.Invoices.AddRangeAsync(invoices);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllInvoicesAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value.Select(x => x.TotalAmount).Should().Contain(new[] { 100m, 200m, 300m });
    }

    [Fact]
    public async Task GetAllInvoicesAsync_WhenNoInvoices_ShouldReturnEmptyList()
    {
        // Act
        var result = await _sut.GetAllInvoicesAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetInvoiceByIdAsync_ShouldReturnCorrectInvoiceData()
    {
        // Arrange
        var invoiceDate = new DateTime(2024, 2, 14, 10, 30, 0, DateTimeKind.Utc);
        var invoice = new Invoice
        {
            InvoiceId = 5,
            OrderId = 10,
            InvoiceDate = invoiceDate,
            TotalAmount = 1500.50m
        };

        await _context.Invoices.AddAsync(invoice);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetInvoiceByIdAsync(5);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.InvoiceId.Should().Be(5);
        result.Value.OrderId.Should().Be(10);
        result.Value.InvoiceDate.Should().Be(invoiceDate);
        result.Value.TotalAmount.Should().Be(1500.50m);
    }

    [Fact]
    public async Task GetAllInvoicesAsync_ShouldReturnInvoicesInCorrectFormat()
    {
        // Arrange
        var invoice1Date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var invoice2Date = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc);

        var invoices = new List<Invoice>
        {
            new Invoice { InvoiceId = 10, OrderId = 100, InvoiceDate = invoice1Date, TotalAmount = 999.99m },
            new Invoice { InvoiceId = 20, OrderId = 200, InvoiceDate = invoice2Date, TotalAmount = 1234.56m }
        };

        await _context.Invoices.AddRangeAsync(invoices);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllInvoicesAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        
        var firstInvoice = result.Value.First(x => x.InvoiceId == 10);
        firstInvoice.OrderId.Should().Be(100);
        firstInvoice.TotalAmount.Should().Be(999.99m);

        var secondInvoice = result.Value.First(x => x.InvoiceId == 20);
        secondInvoice.OrderId.Should().Be(200);
        secondInvoice.TotalAmount.Should().Be(1234.56m);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
