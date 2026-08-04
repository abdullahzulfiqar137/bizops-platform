using BusinessOperations.API.Entities.Inventory;
using BusinessOperations.API.Enums;

namespace BusinessOperations.API.Interfaces.Repositories;

public interface IInventoryMovementRepository : IGenericRepository<InventoryMovement>
{
    Task<(IEnumerable<InventoryMovement> Items, int TotalCount)> GetPagedMovementsAsync(
        Guid? productId,
        InventoryMovementType? movementType,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<string> GenerateNextMovementNumberAsync(CancellationToken cancellationToken = default);
}
