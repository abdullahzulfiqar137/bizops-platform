using BusinessOperations.API.Enums;

namespace BusinessOperations.API.DTOs.Accounting;

public sealed class CreateJournalRequest
{
    public DateTime? EntryDateUtc { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<CreateJournalLineRequest> Lines { get; set; } = [];
}
public sealed class CreateJournalLineRequest { public Guid AccountId { get; set; } public decimal Debit { get; set; } public decimal Credit { get; set; } public string? Memo { get; set; } }
public sealed class AccountResponse { public Guid Id { get; set; } public string Code { get; set; } = ""; public string Name { get; set; } = ""; public AccountType Type { get; set; } public bool IsSystem { get; set; } }
public sealed class JournalResponse { public Guid Id { get; set; } public string EntryNumber { get; set; } = ""; public DateTime EntryDateUtc { get; set; } public string Description { get; set; } = ""; public string ReferenceType { get; set; } = ""; public bool IsAutomatic { get; set; } public decimal Total { get; set; } }
public sealed class LedgerLineResponse { public DateTime DateUtc { get; set; } public string EntryNumber { get; set; } = ""; public string Description { get; set; } = ""; public decimal Debit { get; set; } public decimal Credit { get; set; } public decimal Balance { get; set; } }
public sealed class TrialBalanceLineResponse { public string AccountCode { get; set; } = ""; public string AccountName { get; set; } = ""; public AccountType AccountType { get; set; } public decimal Debit { get; set; } public decimal Credit { get; set; } public decimal Balance { get; set; } }
public sealed class TrialBalanceResponse { public DateTime AsOfUtc { get; set; } public IReadOnlyList<TrialBalanceLineResponse> Lines { get; set; } = []; public decimal TotalDebits { get; set; } public decimal TotalCredits { get; set; } public bool IsBalanced => TotalDebits == TotalCredits; }
public sealed class BalanceSheetResponse { public DateTime AsOfUtc { get; set; } public decimal Assets { get; set; } public decimal Liabilities { get; set; } public decimal Equity { get; set; } public decimal RetainedEarnings { get; set; } public decimal LiabilitiesAndEquity => Liabilities + Equity + RetainedEarnings; public bool IsBalanced => Math.Abs(Assets - LiabilitiesAndEquity) < 0.01m; }
