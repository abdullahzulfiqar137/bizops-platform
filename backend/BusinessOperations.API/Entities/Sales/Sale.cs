using BusinessOperations.API.Entities.Common;
using BusinessOperations.API.Entities.Customers;
using BusinessOperations.API.Enums;

namespace BusinessOperations.API.Entities.Sales;

public sealed class Sale : BaseAuditableEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public DateTime SaleDateUtc { get; set; } = DateTime.UtcNow;

    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }

    public SaleStatus Status { get; set; } = SaleStatus.Completed;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

    public string? Notes { get; set; }

    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    public ICollection<SaleReturn> Returns { get; set; } = new List<SaleReturn>();
}
