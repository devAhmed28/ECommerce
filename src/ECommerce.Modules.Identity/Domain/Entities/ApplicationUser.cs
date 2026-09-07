using Microsoft.AspNetCore.Identity;

namespace ECommerce.Modules.Identity.Domain.Entities;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}