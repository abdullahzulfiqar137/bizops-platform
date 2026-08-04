using BusinessOperations.API.Entities.Common;
using BusinessOperations.API.Entities.Customers;
using BusinessOperations.API.Entities.Purchases;
using BusinessOperations.API.Entities.Sales;
using BusinessOperations.API.Entities.Suppliers;
using BusinessOperations.API.Enums;

namespace BusinessOperations.API.Entities.Payments;

public sealed class Payment : BaseAuditableEntity
{
    public string PaymentNumber { get; set; } = string.Empty;
    public PaymentType PaymentType { get; set; }

    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public Guid? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public Guid? SaleId { get; set; }
    public Sale? Sale { get; set; }

    public Guid? PurchaseId { get; set; }
    public Purchase? Purchase { get; set; }

    public decimal Amount { get; set; }
    public DateTime PaymentDateUtc { get; set; } = DateTime.UtcNow;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}
