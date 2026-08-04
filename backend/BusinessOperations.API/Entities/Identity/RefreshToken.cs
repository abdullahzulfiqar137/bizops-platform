using BusinessOperations.API.Entities.Common;

namespace BusinessOperations.API.Entities.Identity;

public sealed class RefreshToken : BaseEntity, IGlobalEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string CreatedByIp { get; set; } = string.Empty;
    public string? UserAgent { get; set; }

    public DateTime? RevokedAtUtc { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }

    public bool IsActive => RevokedAtUtc == null && !IsExpired;
}
