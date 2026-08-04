using BusinessOperations.API.Entities.Identity;

namespace BusinessOperations.API.Entities.Tenancy;

public sealed class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}
