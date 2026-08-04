using BusinessOperations.API.Entities.Common;

namespace BusinessOperations.API.Entities.Accounting;

public sealed class JournalEntry : BaseAuditableEntity
{
    public string EntryNumber { get; set; } = string.Empty;
    public DateTime EntryDateUtc { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = "Manual";
    public Guid? ReferenceId { get; set; }
    public bool IsAutomatic { get; set; }
    public ICollection<JournalLine> Lines { get; set; } = new List<JournalLine>();
}
