namespace BusinessOperations.API.DTOs.Products;

public sealed class ProductUnitResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
}
