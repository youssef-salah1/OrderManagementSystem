using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Abstractions;
using OrderManagementSystem.Service;

namespace OrderManagementSystem.Controllers;

[Route("api/invoices")]
[ApiController]
[Authorize(Roles = "Admin")]
public class InvoicesController(IInvoiceService invoiceService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await invoiceService.GetInvoiceByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await invoiceService.GetAllInvoicesAsync();
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}