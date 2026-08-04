namespace BusinessOperations.API.DTOs.Reports;

public sealed class ProfitAndLossReportResponse
{
    public decimal GrossSales { get; set; }
    public decimal TotalSalesDiscounts { get; set; }
    public decimal NetSalesRevenue { get; set; }
    public decimal CostOfGoodsSold { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal TotalOperatingExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public decimal ProfitMarginPercentage { get; set; }
}
