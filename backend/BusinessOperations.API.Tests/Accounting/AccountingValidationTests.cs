using BusinessOperations.API.DTOs.Accounting;
using BusinessOperations.API.Validators.Accounting;
using Xunit;

namespace BusinessOperations.API.Tests.Accounting;

public sealed class AccountingValidationTests
{
    private readonly CreateJournalRequestValidator _validator = new();

    [Fact]
    public void Balanced_journal_is_valid()
    {
        var request = Journal(100, 100);
        Assert.True(_validator.Validate(request).IsValid);
    }

    [Fact]
    public void Unbalanced_journal_is_rejected()
    {
        var result = _validator.Validate(Journal(100, 90));
        Assert.Contains(result.Errors, x => x.ErrorMessage.Contains("balance"));
    }

    [Fact]
    public void Debit_and_credit_on_same_line_is_rejected()
    {
        var request = Journal(100, 100);
        request.Lines[0].Credit = 1;
        Assert.False(_validator.Validate(request).IsValid);
    }

    [Fact]
    public void Trial_balance_reports_balance_correctly()
    {
        Assert.True(new TrialBalanceResponse { TotalDebits = 250, TotalCredits = 250 }.IsBalanced);
        Assert.False(new TrialBalanceResponse { TotalDebits = 250, TotalCredits = 249 }.IsBalanced);
    }

    [Fact]
    public void Balance_sheet_allows_rounding_tolerance()
    {
        Assert.True(new BalanceSheetResponse { Assets = 100, Liabilities = 40, Equity = 40, RetainedEarnings = 19.995m }.IsBalanced);
    }

    private static CreateJournalRequest Journal(decimal debit, decimal credit) => new()
    {
        Description = "Opening adjustment",
        Lines =
        [
            new() { AccountId = Guid.NewGuid(), Debit = debit },
            new() { AccountId = Guid.NewGuid(), Credit = credit }
        ]
    };
}
