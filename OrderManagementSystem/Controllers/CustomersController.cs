using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Abstractions;
using OrderManagementSystem.Contracts.Customer;
using OrderManagementSystem.Service;

namespace OrderManagementSystem.Controllers;

[Route("api/customers")]
[ApiController]
[Authorize]
public class CustomersController(ICustomerService customerService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomerCreateRequest request)
    {
        var result = await customerService.CreateCustomerAsync(request);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetOrders), new { id = result.Value.CustomerId }, result.Value)
            : result.ToProblem();
    }

    [HttpGet("{id}/orders")]
    public async Task<IActionResult> GetOrders(int id)
    {
        var result = await customerService.GetCustomerOrdersAsync(id);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}