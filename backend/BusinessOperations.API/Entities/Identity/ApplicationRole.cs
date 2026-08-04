using Microsoft.AspNetCore.Identity;

namespace BusinessOperations.API.Entities.Identity;

public sealed class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();

    public ApplicationRole() : base()
    {
    }

    public ApplicationRole(string roleName, string? description = null) : base(roleName)
    {
        Description = description;
    }
}
