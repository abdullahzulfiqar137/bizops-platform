using BusinessOperations.API.Constants;
using BusinessOperations.API.Data;
using BusinessOperations.API.DTOs.Accounting;
using BusinessOperations.API.Entities.Accounting;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Services.Accounting;

public sealed class AccountingService : IAccountingService
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentUserService _user;
    public AccountingService(ApplicationDbContext db, ICurrentUserService user) { _db = db; _user = user; }

    public async Task<ApiResponse<IReadOnlyList<AccountResponse>>> GetAccountsAsync(CancellationToken token = default)
    {
        var data = await _db.Accounts.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Code)
            .Select(x => new AccountResponse { Id = x.Id, Code = x.Code, Name = x.Name, Type = x.Type, IsSystem = x.IsSystem }).ToListAsync(token);
        return ApiResponse<IReadOnlyList<AccountResponse>>.SuccessResponse(data, "Chart of accounts retrieved successfully.");
    }

    public async Task<ApiResponse<JournalResponse>> CreateManualJournalAsync(CreateJournalRequest request, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(request.Description) || request.Lines.Count < 2)
            return ApiResponse<JournalResponse>.FailureResponse("A description and at least two journal lines are required.");
        if (request.Lines.Any(x => x.Debit < 0 || x.Credit < 0 || (x.Debit > 0) == (x.Credit > 0)))
            return ApiResponse<JournalResponse>.FailureResponse("Each line must contain either a positive debit or a positive credit.");
        var debit = request.Lines.Sum(x => x.Debit); var credit = request.Lines.Sum(x => x.Credit);
        if (debit <= 0 || Math.Abs(debit - credit) >= 0.01m)
            return ApiResponse<JournalResponse>.FailureResponse("Journal debits and credits must be equal.");
        var accountIds = request.Lines.Select(x => x.AccountId).Distinct().ToList();
        if (await _db.Accounts.CountAsync(x => accountIds.Contains(x.Id) && x.IsActive, token) != accountIds.Count)
            return ApiResponse<JournalResponse>.FailureResponse("One or more selected accounts are invalid.");
        var entry = BuildEntry("Manual", null, request.EntryDateUtc ?? DateTime.UtcNow, request.Description.Trim(), false,
            request.Lines.Select(x => (x.AccountId, x.Debit, x.Credit, x.Memo)));
        _db.JournalEntries.Add(entry); await _db.SaveChangesAsync(token);
        return ApiResponse<JournalResponse>.SuccessResponse(Map(entry), "Journal entry posted successfully.");
    }

    public async Task<ApiResponse<IReadOnlyList<JournalResponse>>> GetJournalsAsync(DateTime? start, DateTime? end, CancellationToken token = default)
    {
        await SynchronizeAutomaticEntriesAsync(token);
        var query = _db.JournalEntries.AsNoTracking().Include(x => x.Lines).AsQueryable();
        if (start.HasValue) query = query.Where(x => x.EntryDateUtc >= start);
        if (end.HasValue) query = query.Where(x => x.EntryDateUtc <= end);
        var data = await query.OrderByDescending(x => x.EntryDateUtc).Take(1000).ToListAsync(token);
        return ApiResponse<IReadOnlyList<JournalResponse>>.SuccessResponse(data.Select(Map).ToList(), "Journal entries retrieved successfully.");
    }

    public async Task<ApiResponse<IReadOnlyList<LedgerLineResponse>>> GetLedgerAsync(Guid accountId, DateTime? start, DateTime? end, CancellationToken token = default)
    {
        await SynchronizeAutomaticEntriesAsync(token);
        var query = _db.JournalLines.AsNoTracking().Include(x => x.JournalEntry).Where(x => x.AccountId == accountId);
        if (start.HasValue) query = query.Where(x => x.JournalEntry.EntryDateUtc >= start);
        if (end.HasValue) query = query.Where(x => x.JournalEntry.EntryDateUtc <= end);
        var lines = await query.OrderBy(x => x.JournalEntry.EntryDateUtc).ThenBy(x => x.Id).ToListAsync(token);
        decimal balance = 0; var data = lines.Select(x => { balance += x.Debit - x.Credit; return new LedgerLineResponse { DateUtc = x.JournalEntry.EntryDateUtc, EntryNumber = x.JournalEntry.EntryNumber, Description = x.JournalEntry.Description, Debit = x.Debit, Credit = x.Credit, Balance = balance }; }).ToList();
        return ApiResponse<IReadOnlyList<LedgerLineResponse>>.SuccessResponse(data, "General ledger retrieved successfully.");
    }

    public async Task<ApiResponse<TrialBalanceResponse>> GetTrialBalanceAsync(DateTime? asOf, CancellationToken token = default)
    {
        await SynchronizeAutomaticEntriesAsync(token); var date = asOf ?? DateTime.UtcNow;
        var lines = await _db.Accounts.AsNoTracking().Where(x => x.IsActive).Select(x => new TrialBalanceLineResponse
        {
            AccountCode = x.Code, AccountName = x.Name, AccountType = x.Type,
            Debit = x.JournalLines.Where(y => y.JournalEntry.EntryDateUtc <= date).Sum(y => y.Debit),
            Credit = x.JournalLines.Where(y => y.JournalEntry.EntryDateUtc <= date).Sum(y => y.Credit)
        }).OrderBy(x => x.AccountCode).ToListAsync(token);
        foreach (var line in lines) line.Balance = line.Debit - line.Credit;
        var result = new TrialBalanceResponse { AsOfUtc = date, Lines = lines, TotalDebits = lines.Sum(x => x.Debit), TotalCredits = lines.Sum(x => x.Credit) };
        return ApiResponse<TrialBalanceResponse>.SuccessResponse(result, "Trial balance generated successfully.");
    }

    public async Task<ApiResponse<BalanceSheetResponse>> GetBalanceSheetAsync(DateTime? asOf, CancellationToken token = default)
    {
        var trial = (await GetTrialBalanceAsync(asOf, token)).Data!;
        var assets = trial.Lines.Where(x => x.AccountType == AccountType.Asset).Sum(x => x.Balance);
        var liabilities = -trial.Lines.Where(x => x.AccountType == AccountType.Liability).Sum(x => x.Balance);
        var equity = -trial.Lines.Where(x => x.AccountType == AccountType.Equity).Sum(x => x.Balance);
        var revenue = -trial.Lines.Where(x => x.AccountType == AccountType.Revenue).Sum(x => x.Balance);
        var expenses = trial.Lines.Where(x => x.AccountType == AccountType.Expense).Sum(x => x.Balance);
        var result = new BalanceSheetResponse { AsOfUtc = trial.AsOfUtc, Assets = assets, Liabilities = liabilities, Equity = equity, RetainedEarnings = revenue - expenses };
        return ApiResponse<BalanceSheetResponse>.SuccessResponse(result, "Balance sheet generated successfully.");
    }

    public async Task SynchronizeAutomaticEntriesAsync(CancellationToken token = default)
    {
        var existing = await _db.JournalEntries.AsNoTracking().Where(x => x.ReferenceId != null).Select(x => new { x.ReferenceType, x.ReferenceId }).ToListAsync(token);
        var keys = existing.Select(x => $"{x.ReferenceType}:{x.ReferenceId}").ToHashSet();
        var entries = new List<JournalEntry>();
        var customerPayments = await _db.Payments.AsNoTracking().Where(x => x.SaleId != null)
            .GroupBy(x => x.SaleId!.Value).Select(x => new { Id = x.Key, Total = x.Sum(y => y.Amount) }).ToDictionaryAsync(x => x.Id, x => x.Total, token);
        var supplierPayments = await _db.Payments.AsNoTracking().Where(x => x.PurchaseId != null)
            .GroupBy(x => x.PurchaseId!.Value).Select(x => new { Id = x.Key, Total = x.Sum(y => y.Amount) }).ToDictionaryAsync(x => x.Id, x => x.Total, token);
        var sales = await _db.Sales.AsNoTracking().Include(x => x.SaleItems).ThenInclude(x => x.Product)
            .Where(x => x.Status != SaleStatus.Draft && x.Status != SaleStatus.Cancelled).ToListAsync(token);
        foreach (var x in sales.Where(x => !keys.Contains($"Sale:{x.Id}")))
        {
            var initialPaid = Math.Max(0, x.PaidAmount - customerPayments.GetValueOrDefault(x.Id));
            var initialDue = Math.Max(0, x.TotalAmount - initialPaid);
            var cogs = Math.Round(x.SaleItems.Sum(i => i.Quantity * i.Product.CostPrice), 2);
            entries.Add(BuildEntry("Sale", x.Id, x.SaleDateUtc, $"Sales invoice {x.InvoiceNumber}", true,
                [(SystemAccounts.Cash, initialPaid, 0, null), (SystemAccounts.AccountsReceivable, initialDue, 0, null),
                 (SystemAccounts.SalesRevenue, 0, x.TotalAmount, null), (SystemAccounts.CostOfGoodsSold, cogs, 0, "Cost of goods sold"),
                 (SystemAccounts.Inventory, 0, cogs, "Inventory issued")]));
        }
        var purchases = await _db.Purchases.AsNoTracking().Where(x => x.Status != PurchaseStatus.Draft && x.Status != PurchaseStatus.Cancelled).ToListAsync(token);
        foreach (var x in purchases.Where(x => !keys.Contains($"Purchase:{x.Id}")))
        {
            var initialPaid = Math.Max(0, x.PaidAmount - supplierPayments.GetValueOrDefault(x.Id));
            var initialDue = Math.Max(0, x.TotalAmount - initialPaid);
            entries.Add(BuildEntry("Purchase", x.Id, x.PurchaseDateUtc, $"Purchase order {x.PurchaseOrderNumber}", true,
                [(SystemAccounts.Inventory, x.TotalAmount, 0, null), (SystemAccounts.Cash, 0, initialPaid, null), (SystemAccounts.AccountsPayable, 0, initialDue, null)]));
        }
        var expenses = await _db.Expenses.AsNoTracking().ToListAsync(token);
        foreach (var x in expenses.Where(x => !keys.Contains($"Expense:{x.Id}")))
            entries.Add(BuildEntry("Expense", x.Id, x.ExpenseDateUtc, $"Expense {x.ExpenseNumber}: {x.Title}", true, [(SystemAccounts.OperatingExpenses, x.Amount, 0, null), (SystemAccounts.Cash, 0, x.Amount, null)]));
        var payments = await _db.Payments.AsNoTracking().ToListAsync(token);
        foreach (var x in payments.Where(x => !keys.Contains($"Payment:{x.Id}")))
            entries.Add(x.PaymentType == PaymentType.CustomerPayment
                ? BuildEntry("Payment", x.Id, x.PaymentDateUtc, $"Customer payment {x.PaymentNumber}", true, [(SystemAccounts.Cash, x.Amount, 0, null), (SystemAccounts.AccountsReceivable, 0, x.Amount, null)])
                : BuildEntry("Payment", x.Id, x.PaymentDateUtc, $"Supplier payment {x.PaymentNumber}", true, [(SystemAccounts.AccountsPayable, x.Amount, 0, null), (SystemAccounts.Cash, 0, x.Amount, null)]));
        var saleReturns = await _db.SaleReturns.AsNoTracking().Include(x => x.Items).ThenInclude(x => x.Product).ToListAsync(token);
        foreach (var x in saleReturns.Where(x => !keys.Contains($"SaleReturn:{x.Id}")))
        {
            var restoredCost = Math.Round(x.Items.Where(i => i.Restock).Sum(i => i.Quantity * i.Product.CostPrice), 2);
            entries.Add(BuildEntry("SaleReturn", x.Id, x.ReturnDateUtc, $"Sales return {x.ReturnNumber}", true,
                [(SystemAccounts.SalesReturns, x.TotalAmount, 0, null), (SystemAccounts.AccountsReceivable, 0, x.CustomerCreditApplied, null),
                 (SystemAccounts.Cash, 0, x.RefundAmount, null), (SystemAccounts.Inventory, restoredCost, 0, "Returned inventory"),
                 (SystemAccounts.CostOfGoodsSold, 0, restoredCost, "COGS reversal")]));
        }
        var purchaseReturns = await _db.PurchaseReturns.AsNoTracking().ToListAsync(token);
        foreach (var x in purchaseReturns.Where(x => !keys.Contains($"PurchaseReturn:{x.Id}")))
            entries.Add(BuildEntry("PurchaseReturn", x.Id, x.ReturnDateUtc, $"Purchase return {x.ReturnNumber}", true, [(SystemAccounts.AccountsPayable, x.SupplierCreditApplied, 0, null), (SystemAccounts.Cash, x.RefundAmount, 0, null), (SystemAccounts.Inventory, 0, x.TotalAmount, null)]));
        if (entries.Count > 0) { _db.JournalEntries.AddRange(entries); await _db.SaveChangesAsync(token); }
    }

    private JournalEntry BuildEntry(string type, Guid? referenceId, DateTime date, string description, bool automatic, IEnumerable<(Guid AccountId, decimal Debit, decimal Credit, string? Memo)> lines) =>
        new() { Id = Guid.NewGuid(), EntryNumber = $"JE-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}"[..31].ToUpperInvariant(), EntryDateUtc = date, Description = description, ReferenceType = type, ReferenceId = referenceId, IsAutomatic = automatic, CreatedBy = _user.Username ?? "System", Lines = lines.Where(x => x.Debit != 0 || x.Credit != 0).Select(x => new JournalLine { Id = Guid.NewGuid(), AccountId = x.AccountId, Debit = x.Debit, Credit = x.Credit, Memo = x.Memo }).ToList() };
    private static JournalResponse Map(JournalEntry x) => new() { Id = x.Id, EntryNumber = x.EntryNumber, EntryDateUtc = x.EntryDateUtc, Description = x.Description, ReferenceType = x.ReferenceType, IsAutomatic = x.IsAutomatic, Total = x.Lines.Sum(y => y.Debit) };
}
