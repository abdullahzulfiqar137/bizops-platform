using BusinessOperations.API.Enums;

namespace BusinessOperations.API.DTOs.Purchases;

public sealed class PurchaseResponse
{
    public Guid Id { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string SupplierCompanyName { get; set; } = string.Empty;
    public string SupplierCode { get; set; } = string.Empty;
    public DateTime PurchaseDateUtc { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public PurchaseStatus Status { get; set; }
    public string StatusDescription => Status.ToString();
    public PaymentStatus PaymentStatus { get; set; }
    public string PaymentStatusDescription => PaymentStatus.ToString();
    public string? Notes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public List<PurchaseItemResponse> Items { get; set; } = new();
}
