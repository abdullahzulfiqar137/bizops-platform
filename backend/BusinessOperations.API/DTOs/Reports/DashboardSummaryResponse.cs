namespace BusinessOperations.API.DTOs.Reports;

public sealed class DashboardSummaryResponse
{
    public decimal TotalSalesRevenue { get; set; }
    public decimal TotalPurchases { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public decimal TotalReceivables { get; set; }
    public decimal TotalPayables { get; set; }
    public int LowStockCount { get; set; }
    public decimal TotalInventoryValue { get; set; }
}
