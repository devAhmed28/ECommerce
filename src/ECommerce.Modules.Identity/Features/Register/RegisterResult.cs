using Microsoft.AspNetCore.Identity;

namespace ECommerce.Modules.Identity.Features.Register;
public sealed class RegisterResult
{
    public bool Succeeded { get; init; }
    public RegisterResponse? Response { get; init; }
    public IEnumerable<IdentityError> Errors { get; init; } = Array.Empty<IdentityError>();
    public static RegisterResult Success(RegisterResponse response)
    {
        return new RegisterResult
        {
            Succeeded = true,
            Response = response
        };
    }

    public static RegisterResult Failure(IEnumerable<IdentityError> errors)
    {
        return new RegisterResult
        {
            Succeeded = false,
            Errors = errors
        };
    }
}