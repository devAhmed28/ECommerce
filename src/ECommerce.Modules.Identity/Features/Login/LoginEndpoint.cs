using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ECommerce.Modules.Identity.Features.Login;
[ApiController]
[Route("api/identity/login")]
[EnableRateLimiting("auth")]
public sealed class LoginEndpoint : ControllerBase
{
    private readonly LoginHandler _handler;

    public LoginEndpoint(LoginHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _handler.HandleAsync(command);

        if (!result.Succeeded)
        {
            return Unauthorized(new
            {
                error = result.Error
            });
        }

        return Ok(result);
    }

}
