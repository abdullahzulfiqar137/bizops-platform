using BusinessOperations.API.DTOs.Products;
using BusinessOperations.API.DTOs.Reports;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface IReportService
{
    Task<ApiResponse<DashboardSummaryResponse>> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<SalesSummaryReportResponse>> GetSalesSummaryAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
    Task<ApiResponse<PurchaseSummaryReportResponse>> GetPurchaseSummaryAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
    Task<ApiResponse<ExpenseSummaryReportResponse>> GetExpenseSummaryAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
    Task<ApiResponse<ProfitAndLossReportResponse>> GetProfitAndLossAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<ProductResponse>>> GetLowStockReportAsync(CancellationToken cancellationToken = default);
}
