using BusinessOperations.API.DTOs.Inventory;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface IInventoryService
{
    Task<ApiResponse<InventoryAdjustmentResponse>> CreateAdjustmentAsync(CreateInventoryAdjustmentRequest request, CancellationToken cancellationToken = default);

    Task<PagedResponse<InventoryAdjustmentResponse>> GetAdjustmentsAsync(
        Guid? productId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PagedResponse<InventoryMovementResponse>> GetMovementsAsync(
        Guid? productId,
        InventoryMovementType? movementType,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<IEnumerable<StockLevelResponse>>> GetStockLevelsAsync(Guid? categoryId, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<StockLevelResponse>>> GetLowStockAlertsAsync(CancellationToken cancellationToken = default);
}
