using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Abstractions;
using OrderManagementSystem.Contracts.Product;
using OrderManagementSystem.Service;

namespace OrderManagementSystem.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await productService.GetAllProductsAsync();
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await productService.GetProductByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // Requirement: Admin only
    public async Task<IActionResult> Create([FromBody] ProductCreateRequest request)
    {
        var result = await productService.CreateProductAsync(request);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.ProductId }, result.Value)
            : result.ToProblem();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")] // Requirement: Admin only
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateRequest request)
    {
        var result = await productService.UpdateProductAsync(id, request);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}