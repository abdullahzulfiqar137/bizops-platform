using BusinessOperations.API.Enums;

namespace BusinessOperations.API.DTOs.Payments;

public sealed class CreatePaymentRequest
{
    public PaymentType PaymentType { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? SupplierId { get; set; }
    public Guid? SaleId { get; set; }
    public Guid? PurchaseId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}
