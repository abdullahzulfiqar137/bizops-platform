using BusinessOperations.API.Enums;

namespace BusinessOperations.API.DTOs.Payments;

public sealed class PaymentResponse
{
    public Guid Id { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public PaymentType PaymentType { get; set; }
    public string PaymentTypeDescription => PaymentType.ToString();
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public Guid? SaleId { get; set; }
    public string? InvoiceNumber { get; set; }
    public Guid? PurchaseId { get; set; }
    public string? PurchaseOrderNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDateUtc { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string PaymentMethodDescription => PaymentMethod.ToString();
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
