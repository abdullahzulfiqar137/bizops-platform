namespace BusinessOperations.API.DTOs.Reports;

public sealed class ExpenseCategoryBreakdown
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PercentageOfTotal { get; set; }
}

public sealed class ExpenseSummaryReportResponse
{
    public int TotalExpensesCount { get; set; }
    public decimal TotalExpenseAmount { get; set; }
    public List<ExpenseCategoryBreakdown> CategoryBreakdown { get; set; } = new();
}
