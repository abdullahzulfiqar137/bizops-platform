using BusinessOperations.API.DTOs.Accounting;
using BusinessOperations.API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/accounting")]
[Authorize(Roles = "Admin,Manager")]
public sealed class AccountingController : ControllerBase
{
    private readonly IAccountingService _accounting;
    public AccountingController(IAccountingService accounting) => _accounting = accounting;

    [HttpGet("accounts")]
    public async Task<IActionResult> Accounts(CancellationToken token) => Ok(await _accounting.GetAccountsAsync(token));
    [HttpPost("journals")]
    public async Task<IActionResult> CreateJournal(CreateJournalRequest request, CancellationToken token)
    {
        var response = await _accounting.CreateManualJournalAsync(request, token);
        return response.Success ? Ok(response) : BadRequest(response);
    }
    [HttpGet("journals")]
    public async Task<IActionResult> Journals(DateTime? start, DateTime? end, CancellationToken token) => Ok(await _accounting.GetJournalsAsync(start, end, token));
    [HttpGet("ledger/{accountId:guid}")]
    public async Task<IActionResult> Ledger(Guid accountId, DateTime? start, DateTime? end, CancellationToken token) => Ok(await _accounting.GetLedgerAsync(accountId, start, end, token));
    [HttpGet("trial-balance")]
    public async Task<IActionResult> TrialBalance(DateTime? asOf, CancellationToken token) => Ok(await _accounting.GetTrialBalanceAsync(asOf, token));
    [HttpGet("balance-sheet")]
    public async Task<IActionResult> BalanceSheet(DateTime? asOf, CancellationToken token) => Ok(await _accounting.GetBalanceSheetAsync(asOf, token));
    [HttpPost("synchronize")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Synchronize(CancellationToken token) { await _accounting.SynchronizeAutomaticEntriesAsync(token); return Ok(new { success = true, message = "Accounting entries synchronized." }); }
}
