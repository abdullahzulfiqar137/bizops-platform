namespace BusinessOperations.API.DTOs.Purchases;

public sealed class CreatePurchaseRequest
{
    public Guid SupplierId { get; set; }
    public decimal TaxRate { get; set; } = 0.00m;
    public decimal DiscountAmount { get; set; } = 0.00m;
    public decimal PaidAmount { get; set; } = 0.00m;
    public string? Notes { get; set; }
    public List<CreatePurchaseItemRequest> Items { get; set; } = new();
}
