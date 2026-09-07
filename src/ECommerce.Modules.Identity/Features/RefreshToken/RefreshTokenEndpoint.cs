using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ECommerce.Modules.Identity.Features.RefreshToken;
[ApiController]
[Route("api/identity/refresh")]
[EnableRateLimiting("auth")]
public sealed class RefreshTokenEndpoint : ControllerBase
{
    private readonly RefreshTokenHandler _handler;

	public RefreshTokenEndpoint(RefreshTokenHandler refreshTokenHandler)
    {
        _handler = refreshTokenHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    {
        var result = await _handler.HandleAsync(command);

        if (result.Response is null)
        {
            return Unauthorized(new
            {
                error = "Invalid or expired refresh token."
            });
        }

        return Ok(result.Response);
    }
}