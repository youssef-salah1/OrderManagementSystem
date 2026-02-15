using OrderManagementSystem.Abstractions;
using OrderManagementSystem.Contracts.Product;

namespace OrderManagementSystem.Service;

public interface IProductService
{
    Task<Result<IEnumerable<ProductResponse>>> GetAllProductsAsync();
    Task<Result<ProductResponse>> GetProductByIdAsync(int id);
    Task<Result<ProductResponse>> CreateProductAsync(ProductCreateRequest request);
    Task<Result> UpdateProductAsync(int id, ProductUpdateRequest request);
}