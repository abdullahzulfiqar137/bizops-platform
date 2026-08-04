using BusinessOperations.API.Entities.Common;
using BusinessOperations.API.Entities.Identity;

namespace BusinessOperations.API.Entities.Notifications;

public sealed class UserNotification : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public string SourceKey { get; set; } = string.Empty;
    public string Type { get; set; } = "info";
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Route { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAtUtc { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
