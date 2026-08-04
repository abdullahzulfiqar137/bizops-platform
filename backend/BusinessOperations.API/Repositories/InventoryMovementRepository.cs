using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Inventory;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Repositories;

public sealed class InventoryMovementRepository : GenericRepository<InventoryMovement>, IInventoryMovementRepository
{
    public InventoryMovementRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<InventoryMovement> Items, int TotalCount)> GetPagedMovementsAsync(
        Guid? productId,
        InventoryMovementType? movementType,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(m => m.Product)
            .AsNoTracking();

        if (productId.HasValue)
        {
            query = query.Where(m => m.ProductId == productId.Value);
        }

        if (movementType.HasValue)
        {
            query = query.Where(m => m.MovementType == movementType.Value);
        }

        query = query.OrderByDescending(m => m.CreatedAtUtc);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<string> GenerateNextMovementNumberAsync(CancellationToken cancellationToken = default)
    {
        var count = await DbSet.CountAsync(cancellationToken);
        return $"MOV-{(count + 1):D5}";
    }
}
