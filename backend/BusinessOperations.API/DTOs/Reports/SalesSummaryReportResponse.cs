namespace BusinessOperations.API.DTOs.Reports;

public sealed class MonthlySalesSummary
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int InvoiceCount { get; set; }
    public decimal TotalRevenue { get; set; }
}

public sealed class SalesSummaryReportResponse
{
    public int TotalInvoices { get; set; }
    public decimal GrossSales { get; set; }
    public decimal TotalTaxAmount { get; set; }
    public decimal TotalDiscountAmount { get; set; }
    public decimal NetSalesRevenue { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal TotalDueAmount { get; set; }
    public decimal AverageOrderValue { get; set; }
    public List<MonthlySalesSummary> MonthlyBreakdown { get; set; } = new();
}
