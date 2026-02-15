using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Abstractions;
using OrderManagementSystem.Contracts;
using OrderManagementSystem.Service;

namespace OrderManagementSystem.Controllers;

[Route("api/users")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
    {
        var result = await authService.LoginAsync(request);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }
}