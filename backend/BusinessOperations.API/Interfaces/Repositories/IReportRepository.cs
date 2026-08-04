using BusinessOperations.API.DTOs.Reports;

namespace BusinessOperations.API.Interfaces.Repositories;

public interface IReportRepository
{
    Task<DashboardSummaryResponse> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
    Task<SalesSummaryReportResponse> GetSalesSummaryAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
    Task<PurchaseSummaryReportResponse> GetPurchaseSummaryAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
    Task<ExpenseSummaryReportResponse> GetExpenseSummaryAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
    Task<ProfitAndLossReportResponse> GetProfitAndLossAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
}
