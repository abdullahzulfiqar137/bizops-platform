using BusinessOperations.API.Entities.Common;
using BusinessOperations.API.Enums;

namespace BusinessOperations.API.Entities.Accounting;

public sealed class Account : BaseAuditableEntity, IGlobalEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<JournalLine> JournalLines { get; set; } = new List<JournalLine>();
}
