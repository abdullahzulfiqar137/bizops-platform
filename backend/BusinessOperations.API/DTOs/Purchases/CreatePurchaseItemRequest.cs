namespace BusinessOperations.API.DTOs.Purchases;

public sealed class CreatePurchaseItemRequest
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
}
