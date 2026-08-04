using BusinessOperations.API.Entities.Common;

namespace BusinessOperations.API.Entities.Sales;

public sealed class SaleReturn : BaseAuditableEntity
{
    public string ReturnNumber { get; set; } = string.Empty;
    public Guid SaleId { get; set; }
    public Sale Sale { get; set; } = null!;
    public DateTime ReturnDateUtc { get; set; } = DateTime.UtcNow;
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CustomerCreditApplied { get; set; }
    public decimal RefundAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public ICollection<SaleReturnItem> Items { get; set; } = new List<SaleReturnItem>();
}
