namespace BusinessOperations.API.DTOs.Sales;

public sealed class SaleReturnResponse
{
    public Guid Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public Guid SaleId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime ReturnDateUtc { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CustomerCreditApplied { get; set; }
    public decimal RefundAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public IReadOnlyList<SaleReturnItemResponse> Items { get; set; } = [];
}

public sealed class SaleReturnItemResponse
{
    public Guid SaleItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public bool Restocked { get; set; }
}
