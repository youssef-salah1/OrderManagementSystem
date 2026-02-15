using OrderManagementSystem.Abstractions;
using OrderManagementSystem.Contracts.Product;
using OrderManagementSystem.Entity;
using OrderManagementSystem.Errors;

namespace OrderManagementSystem.Service;

public class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<Result<IEnumerable<ProductResponse>>> GetAllProductsAsync()
    {
        var products = await productRepository.GetAllAsync();
        var response = products.Select(p => new ProductResponse(p.ProductId, p.Name, p.Price, p.Stock));
        return Result.Success(response);
    }

    public async Task<Result<ProductResponse>> GetProductByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null) return Result.Failure<ProductResponse>(ProductErrors.NotFound);
        return Result.Success(new ProductResponse(product.ProductId, product.Name, product.Price, product.Stock));
    }

    public async Task<Result<ProductResponse>> CreateProductAsync(ProductCreateRequest request)
    {
        var product = new Product { Name = request.Name, Price = request.Price, Stock = request.Stock };
        await productRepository.AddAsync(product);
        return Result.Success(new ProductResponse(product.ProductId, product.Name, product.Price, product.Stock));
    }

    public async Task<Result> UpdateProductAsync(int id, ProductUpdateRequest request)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null) return Result.Failure(ProductErrors.NotFound);

        product.Name = request.Name;
        product.Price = request.Price;
        product.Stock = request.Stock;

        await productRepository.UpdateAsync(product);
        return Result.Success();
    }
}