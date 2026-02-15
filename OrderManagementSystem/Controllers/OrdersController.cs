using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Abstractions;
using OrderManagementSystem.Contracts.Order;
using OrderManagementSystem.Service;

namespace OrderManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderCreateRequest request)
    {
        var result = await _orderService.CreateOrderAsync(request);

        if (result.IsSuccess)
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Value.OrderId },
                result.Value
            );

        return result.ToProblem();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _orderService.GetOrderByIdAsync(id);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await orderService.GetAllOrdersAsync();

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatusUpdateRequest request)
    {
        var result = await orderService.UpdateOrderStatusAsync(id, request.Status);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}