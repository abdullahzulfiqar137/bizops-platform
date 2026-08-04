using BusinessOperations.API.Entities.Common;

namespace BusinessOperations.API.Entities.Audit;

public sealed class AuditLog : BaseEntity
{
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = string.Empty; // Create, Update, Delete
    public string EntityName { get; set; } = string.Empty; // e.g., Product, Customer, Sale
    public string EntityId { get; set; } = string.Empty;
    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON
    public string? AffectedColumns { get; set; } // JSON list of columns
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}
