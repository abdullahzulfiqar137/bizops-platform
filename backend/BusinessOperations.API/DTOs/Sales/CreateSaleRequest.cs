namespace BusinessOperations.API.DTOs.Sales;

public sealed class CreateSaleRequest
{
    public Guid CustomerId { get; set; }
    public decimal TaxRate { get; set; } = 0.00m;
    public decimal DiscountAmount { get; set; } = 0.00m;
    public decimal PaidAmount { get; set; } = 0.00m;
    public string? Notes { get; set; }
    public List<CreateSaleItemRequest> Items { get; set; } = new();
}
