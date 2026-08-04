using BusinessOperations.API.Enums;

namespace BusinessOperations.API.DTOs.Sales;

public sealed class SaleResponse
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public DateTime SaleDateUtc { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public SaleStatus Status { get; set; }
    public string StatusDescription => Status.ToString();
    public PaymentStatus PaymentStatus { get; set; }
    public string PaymentStatusDescription => PaymentStatus.ToString();
    public string? Notes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public List<SaleItemResponse> Items { get; set; } = new();
}
