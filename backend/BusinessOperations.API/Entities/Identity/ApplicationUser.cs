using BusinessOperations.API.Entities.Common;
using BusinessOperations.API.Entities.Tenancy;
using Microsoft.AspNetCore.Identity;

namespace BusinessOperations.API.Entities.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>, IAuditableEntity, ISoftDeletable, ITenantOwned
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAtUtc { get; set; }
    public string? DeletedBy { get; set; }

    public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
