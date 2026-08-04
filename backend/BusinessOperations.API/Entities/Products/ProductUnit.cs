using BusinessOperations.API.Entities.Common;

namespace BusinessOperations.API.Entities.Products;

public sealed class ProductUnit : BaseEntity, IGlobalEntity
{
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
