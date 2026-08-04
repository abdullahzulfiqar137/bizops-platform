using BusinessOperations.API.Entities.Common;

namespace BusinessOperations.API.Entities.Purchases;

public sealed class PurchaseReturn : BaseAuditableEntity
{
    public string ReturnNumber { get; set; } = string.Empty;
    public Guid PurchaseId { get; set; }
    public Purchase Purchase { get; set; } = null!;
    public DateTime ReturnDateUtc { get; set; } = DateTime.UtcNow;
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal SupplierCreditApplied { get; set; }
    public decimal RefundAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public ICollection<PurchaseReturnItem> Items { get; set; } = new List<PurchaseReturnItem>();
}
