namespace BusinessOperations.API.DTOs.Suppliers;

public sealed class SupplierPurchaseHistoryResponse
{
    public Guid PurchaseId { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingBalance { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime PurchaseDateUtc { get; set; }
}
