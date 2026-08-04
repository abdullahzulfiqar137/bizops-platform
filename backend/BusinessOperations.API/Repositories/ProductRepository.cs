using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Products;
using BusinessOperations.API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Repositories;

public sealed class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedProductsAsync(
        string? searchTerm,
        Guid? categoryId,
        bool? isActive,
        bool? isLowStock,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim().ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(search) ||
                p.SKU.ToLower().Contains(search) ||
                (p.Barcode != null && p.Barcode.ToLower().Contains(search)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        if (isLowStock.HasValue && isLowStock.Value)
        {
            query = query.Where(p => p.StockQuantity <= p.MinimumStockLevel);
        }

        query = sortBy?.ToLower() switch
        {
            "name" => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "sku" => isDescending ? query.OrderByDescending(p => p.SKU) : query.OrderBy(p => p.SKU),
            "price" => isDescending ? query.OrderByDescending(p => p.SalePrice) : query.OrderBy(p => p.SalePrice),
            "stock" => isDescending ? query.OrderByDescending(p => p.StockQuantity) : query.OrderBy(p => p.StockQuantity),
            _ => isDescending ? query.OrderByDescending(p => p.CreatedAtUtc) : query.OrderBy(p => p.CreatedAtUtc)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Product?> GetProductWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product?> GetBySKUAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(p => p.SKU == sku, cancellationToken);
    }

    public async Task<string> GenerateNextSKUAsync(CancellationToken cancellationToken = default)
    {
        var count = await DbSet.IgnoreQueryFilters()
            .CountAsync(x => x.TenantId == Context.CurrentTenantId, cancellationToken);
        return $"SKU-{(count + 1):D5}";
    }
}
