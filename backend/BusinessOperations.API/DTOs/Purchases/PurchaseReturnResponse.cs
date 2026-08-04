namespace BusinessOperations.API.DTOs.Purchases;

public sealed class PurchaseReturnResponse
{
    public Guid Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public Guid PurchaseId { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public DateTime ReturnDateUtc { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal SupplierCreditApplied { get; set; }
    public decimal RefundAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public IReadOnlyList<PurchaseReturnItemResponse> Items { get; set; } = [];
}

public sealed class PurchaseReturnItemResponse
{
    public Guid PurchaseItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalPrice { get; set; }
}
