namespace ECommerce.Modules.Identity.Features.Register;

public sealed class RegisterResponse
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public bool EmailConfirmed { get; set; }
}