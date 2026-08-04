namespace BusinessOperations.API.DTOs.Reports;

public sealed class PurchaseSummaryReportResponse
{
    public int TotalPurchaseOrders { get; set; }
    public decimal GrossPurchases { get; set; }
    public decimal TotalTaxAmount { get; set; }
    public decimal TotalDiscountAmount { get; set; }
    public decimal NetPurchaseSpend { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal TotalDueAmount { get; set; }
}
