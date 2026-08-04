using BusinessOperations.API.DTOs.Inventory;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>
    /// Records a stock adjustment (addition or deduction).
    /// </summary>
    [HttpPost("adjustments")]
    [ProducesResponseType(typeof(ApiResponse<InventoryAdjustmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<InventoryAdjustmentResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAdjustment([FromBody] CreateInventoryAdjustmentRequest request, CancellationToken cancellationToken)
    {
        var response = await _inventoryService.CreateAdjustmentAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Gets paginated history of stock adjustments.
    /// </summary>
    [HttpGet("adjustments")]
    [ProducesResponseType(typeof(PagedResponse<InventoryAdjustmentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAdjustments(
        [FromQuery] Guid? productId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var response = await _inventoryService.GetAdjustmentsAsync(productId, pageNumber, pageSize, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Gets paginated audit log of all inventory movements.
    /// </summary>
    [HttpGet("movements")]
    [ProducesResponseType(typeof(PagedResponse<InventoryMovementResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMovements(
        [FromQuery] Guid? productId,
        [FromQuery] InventoryMovementType? movementType,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var response = await _inventoryService.GetMovementsAsync(productId, movementType, pageNumber, pageSize, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Gets current stock levels across all products.
    /// </summary>
    [HttpGet("stock-levels")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StockLevelResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStockLevels([FromQuery] Guid? categoryId, CancellationToken cancellationToken)
    {
        var response = await _inventoryService.GetStockLevelsAsync(categoryId, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Gets low stock alert products where stock quantity is at or below minimum stock level.
    /// </summary>
    [HttpGet("low-stock-alerts")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StockLevelResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLowStockAlerts(CancellationToken cancellationToken)
    {
        var response = await _inventoryService.GetLowStockAlertsAsync(cancellationToken);
        return Ok(response);
    }
}
