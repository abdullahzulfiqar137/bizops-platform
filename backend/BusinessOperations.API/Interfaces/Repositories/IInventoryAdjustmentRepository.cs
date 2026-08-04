using BusinessOperations.API.Entities.Inventory;

namespace BusinessOperations.API.Interfaces.Repositories;

public interface IInventoryAdjustmentRepository : IGenericRepository<InventoryAdjustment>
{
    Task<(IEnumerable<InventoryAdjustment> Items, int TotalCount)> GetPagedAdjustmentsAsync(
        Guid? productId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<string> GenerateNextAdjustmentNumberAsync(CancellationToken cancellationToken = default);
}
