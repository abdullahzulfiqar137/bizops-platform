using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Inventory;
using BusinessOperations.API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Repositories;

public sealed class InventoryAdjustmentRepository : GenericRepository<InventoryAdjustment>, IInventoryAdjustmentRepository
{
    public InventoryAdjustmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<InventoryAdjustment> Items, int TotalCount)> GetPagedAdjustmentsAsync(
        Guid? productId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(a => a.Product)
            .AsNoTracking();

        if (productId.HasValue)
        {
            query = query.Where(a => a.ProductId == productId.Value);
        }

        query = query.OrderByDescending(a => a.CreatedAtUtc);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<string> GenerateNextAdjustmentNumberAsync(CancellationToken cancellationToken = default)
    {
        var count = await DbSet.CountAsync(cancellationToken);
        return $"ADJ-{(count + 1):D5}";
    }
}
