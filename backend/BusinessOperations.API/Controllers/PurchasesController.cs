using BusinessOperations.API.DTOs.Purchases;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchasesController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    /// <summary>
    /// Creates a new purchase order. Automatically updates product stock and supplier balance.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PurchaseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PurchaseResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePurchase([FromBody] CreatePurchaseRequest request, CancellationToken cancellationToken)
    {
        var response = await _purchaseService.CreatePurchaseAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Gets purchase order list with search, filtering by supplier/status/date, sorting, and pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<PurchaseResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPurchases(
        [FromQuery] string? search,
        [FromQuery] Guid? supplierId,
        [FromQuery] PurchaseStatus? status,
        [FromQuery] PaymentStatus? paymentStatus,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? sortBy,
        [FromQuery] bool isDescending = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var response = await _purchaseService.GetPurchasesAsync(
            search, supplierId, status, paymentStatus, startDate, endDate, sortBy, isDescending, pageNumber, pageSize, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Gets purchase order details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PurchaseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PurchaseResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPurchaseById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _purchaseService.GetPurchaseByIdAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Cancels a purchase order, reverting product stock and supplier balance.
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelPurchase(Guid id, CancellationToken cancellationToken)
    {
        var response = await _purchaseService.CancelPurchaseAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
