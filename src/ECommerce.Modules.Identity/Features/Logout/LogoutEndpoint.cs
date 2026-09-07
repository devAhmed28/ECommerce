using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace ECommerce.Modules.Identity.Features.Logout;
[ApiController]
[Route("api/identity/logout")]
public sealed class LogoutEndpoint : ControllerBase
{
    private readonly LogoutHandler _handler;

    public LogoutEndpoint(LogoutHandler logoutHandler)
    {
        _handler = logoutHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutCommand command)
    {
        var succeeded = await _handler.HandleAsync(command);

        if (!succeeded)
        {
            return Unauthorized(new
            {
                error = "Invalid or already revoked refresh token."
            });
        }

        return NoContent();
    }
}