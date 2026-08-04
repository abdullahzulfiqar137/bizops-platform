using BusinessOperations.API.DTOs.Accounting;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface IAccountingService
{
    Task<ApiResponse<IReadOnlyList<AccountResponse>>> GetAccountsAsync(CancellationToken token = default);
    Task<ApiResponse<JournalResponse>> CreateManualJournalAsync(CreateJournalRequest request, CancellationToken token = default);
    Task<ApiResponse<IReadOnlyList<JournalResponse>>> GetJournalsAsync(DateTime? start, DateTime? end, CancellationToken token = default);
    Task<ApiResponse<IReadOnlyList<LedgerLineResponse>>> GetLedgerAsync(Guid accountId, DateTime? start, DateTime? end, CancellationToken token = default);
    Task<ApiResponse<TrialBalanceResponse>> GetTrialBalanceAsync(DateTime? asOf, CancellationToken token = default);
    Task<ApiResponse<BalanceSheetResponse>> GetBalanceSheetAsync(DateTime? asOf, CancellationToken token = default);
    Task SynchronizeAutomaticEntriesAsync(CancellationToken token = default);
}
