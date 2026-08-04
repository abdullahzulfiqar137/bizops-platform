using BusinessOperations.API.Entities.Common;
using BusinessOperations.API.Entities.Suppliers;
using BusinessOperations.API.Enums;

namespace BusinessOperations.API.Entities.Purchases;

public sealed class Purchase : BaseAuditableEntity
{
    public string PurchaseOrderNumber { get; set; } = string.Empty;

    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public DateTime PurchaseDateUtc { get; set; } = DateTime.UtcNow;

    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }

    public PurchaseStatus Status { get; set; } = PurchaseStatus.Completed;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

    public string? Notes { get; set; }

    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    public ICollection<PurchaseReturn> Returns { get; set; } = new List<PurchaseReturn>();
}
