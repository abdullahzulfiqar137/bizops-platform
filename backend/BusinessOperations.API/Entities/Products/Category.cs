using BusinessOperations.API.Entities.Common;

namespace BusinessOperations.API.Entities.Products;

public sealed class Category : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
