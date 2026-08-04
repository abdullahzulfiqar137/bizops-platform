using Microsoft.AspNetCore.Identity;

namespace BusinessOperations.API.Entities.Identity;

public sealed class ApplicationUserRole : IdentityUserRole<Guid>
{
    public ApplicationUser User { get; set; } = null!;
    public ApplicationRole Role { get; set; } = null!;
}
