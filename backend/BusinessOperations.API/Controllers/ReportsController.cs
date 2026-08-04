using BusinessOperations.API.DTOs.Products;
using BusinessOperations.API.DTOs.Reports;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Gets real-time executive dashboard KPIs summary (Revenue, Expenses, Net Profit, Receivables, Payables, Inventory Value).
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(ApiResponse<DashboardSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardSummary(CancellationToken cancellationToken)
    {
        var response = await _reportService.GetDashboardSummaryAsync(cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Gets sales performance summary report with optional date range filter and monthly trend breakdown.
    /// </summary>
    [HttpGet("sales-summary")]
    [ProducesResponseType(typeof(ApiResponse<SalesSummaryReportResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSalesSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken cancellationToken)
    {
        var response = await _reportService.GetSalesSummaryAsync(startDate, endDate, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Gets purchase spend summary report with optional date range filter.
    /// </summary>
    [HttpGet("purchase-summary")]
    [ProducesResponseType(typeof(ApiResponse<PurchaseSummaryReportResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPurchaseSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken cancellationToken)
    {
        var response = await _reportService.GetPurchaseSummaryAsync(startDate, endDate, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Gets operational expense summary report with category distribution percentages.
    /// </summary>
    [HttpGet("expense-summary")]
    [ProducesResponseType(typeof(ApiResponse<ExpenseSummaryReportResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExpenseSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken cancellationToken)
    {
        var response = await _reportService.GetExpenseSummaryAsync(startDate, endDate, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Gets Profit and Loss (P&amp;L) financial statement (Net Sales, COGS, Gross Profit, Expenses, Net Profit Margin).
    /// </summary>
    [HttpGet("profit-and-loss")]
    [ProducesResponseType(typeof(ApiResponse<ProfitAndLossReportResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfitAndLoss([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken cancellationToken)
    {
        var response = await _reportService.GetProfitAndLossAsync(startDate, endDate, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Gets inventory products that are at or below their minimum stock threshold.
    /// </summary>
    [HttpGet("low-stock")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLowStockReport(CancellationToken cancellationToken)
    {
        var response = await _reportService.GetLowStockReportAsync(cancellationToken);
        return Ok(response);
    }
}
