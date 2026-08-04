namespace BusinessOperations.API.Entities.Common;

public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity, ISoftDeletable
{
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAtUtc { get; set; }
    public string? DeletedBy { get; set; }
}
