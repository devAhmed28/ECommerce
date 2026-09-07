using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Modules.Identity.Features.ConfirmEmail;
[ApiController]
[Route("api/identity/confirm-email")]
public sealed class ConfirmEmailEndpoint : ControllerBase
{
    private readonly ConfirmEmailHandler _handler;

	public ConfirmEmailEndpoint(ConfirmEmailHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
    {
        var command = new ConfirmEmailCommand(userId, token);

        var result = await _handler.HandleAsync(command);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                error = result.Error
            });
        }

        return Ok(new
        {
            message = "Email confirmed successfully."
        });
    }
}