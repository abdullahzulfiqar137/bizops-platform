using BusinessOperations.API.Entities.Products;

namespace BusinessOperations.API.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedProductsAsync(
        string? searchTerm,
        Guid? categoryId,
        bool? isActive,
        bool? isLowStock,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Product?> GetProductWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetBySKUAsync(string sku, CancellationToken cancellationToken = default);
    Task<string> GenerateNextSKUAsync(CancellationToken cancellationToken = default);
}
