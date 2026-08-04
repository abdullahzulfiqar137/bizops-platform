using BusinessOperations.API.Entities.Common;

namespace BusinessOperations.API.Entities.Accounting;

public sealed class JournalLine : BaseEntity
{
    public Guid JournalEntryId { get; set; }
    public JournalEntry JournalEntry { get; set; } = null!;
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public string? Memo { get; set; }
}
