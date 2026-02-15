using FluentAssertions;
using Moq;
using OrderManagementSystem.Contracts.Product;
using OrderManagementSystem.Entity;
using OrderManagementSystem.Service;

namespace OrderManagementSystem.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _sut = new ProductService(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { ProductId = 1, Name = "Laptop", Price = 1500m, Stock = 10 },
            new Product { ProductId = 2, Name = "Mouse", Price = 50m, Stock = 100 },
            new Product { ProductId = 3, Name = "Keyboard", Price = 100m, Stock = 50 }
        };

        _productRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _sut.GetAllProductsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value.First().Name.Should().Be("Laptop");
        result.Value.Last().Name.Should().Be("Keyboard");
    }

    [Fact]
    public async Task GetAllProductsAsync_WhenNoProducts_ShouldReturnEmptyList()
    {
        // Arrange
        _productRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Product>());

        // Act
        var result = await _sut.GetAllProductsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenProductExists_ShouldReturnProduct()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            Name = "Laptop",
            Price = 1500m,
            Stock = 10
        };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _sut.GetProductByIdAsync(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ProductId.Should().Be(1);
        result.Value.Name.Should().Be("Laptop");
        result.Value.Price.Should().Be(1500m);
        result.Value.Stock.Should().Be(10);
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenProductDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.GetProductByIdAsync(999);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.NotFound");
    }

    [Fact]
    public async Task CreateProductAsync_WhenValidRequest_ShouldCreateProduct()
    {
        // Arrange
        var request = new ProductCreateRequest("New Laptop", 2000m, 15);

        _productRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateProductAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("New Laptop");
        result.Value.Price.Should().Be(2000m);
        result.Value.Stock.Should().Be(15);
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldSetAllProductProperties()
    {
        // Arrange
        var request = new ProductCreateRequest("Gaming Mouse", 79.99m, 200);

        Product? capturedProduct = null;
        _productRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Product>()))
            .Callback<Product>(product => capturedProduct = product)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateProductAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedProduct.Should().NotBeNull();
        capturedProduct!.Name.Should().Be("Gaming Mouse");
        capturedProduct.Price.Should().Be(79.99m);
        capturedProduct.Stock.Should().Be(200);
    }

    [Fact]
    public async Task UpdateProductAsync_WhenProductDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var request = new ProductUpdateRequest("Updated Product", 1000m, 50);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.UpdateProductAsync(999, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.NotFound");
    }

    [Fact]
    public async Task UpdateProductAsync_WhenProductExists_ShouldUpdateProduct()
    {
        // Arrange
        var existingProduct = new Product
        {
            ProductId = 1,
            Name = "Old Name",
            Price = 500m,
            Stock = 10
        };

        var request = new ProductUpdateRequest("Updated Name", 600m, 20);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingProduct);

        _productRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateProductAsync(1, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingProduct.Name.Should().Be("Updated Name");
        existingProduct.Price.Should().Be(600m);
        existingProduct.Stock.Should().Be(20);
        _productRepositoryMock.Verify(x => x.UpdateAsync(existingProduct), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldUpdateAllProperties()
    {
        // Arrange
        var existingProduct = new Product
        {
            ProductId = 1,
            Name = "Original Keyboard",
            Price = 100m,
            Stock = 50
        };

        var request = new ProductUpdateRequest("Mechanical Keyboard Pro", 150m, 75);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingProduct);

        _productRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateProductAsync(1, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingProduct.Name.Should().Be("Mechanical Keyboard Pro");
        existingProduct.Price.Should().Be(150m);
        existingProduct.Stock.Should().Be(75);
    }
}
