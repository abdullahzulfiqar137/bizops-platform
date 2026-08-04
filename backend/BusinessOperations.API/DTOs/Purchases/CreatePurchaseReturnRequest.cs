namespace BusinessOperations.API.DTOs.Purchases;

public sealed class CreatePurchaseReturnRequest
{
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<CreatePurchaseReturnItemRequest> Items { get; set; } = [];
}

public sealed class CreatePurchaseReturnItemRequest
{
    public Guid PurchaseItemId { get; set; }
    public decimal Quantity { get; set; }
}
