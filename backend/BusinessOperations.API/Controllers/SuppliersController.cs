using BusinessOperations.API.DTOs.Suppliers;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    /// <summary>
    /// Gets supplier list with search, filtering, sorting, and pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<SupplierResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSuppliers(
        [FromQuery] string? search,
        [FromQuery] string? city,
        [FromQuery] bool? isActive,
        [FromQuery] string? sortBy,
        [FromQuery] bool isDescending = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var response = await _supplierService.GetSuppliersAsync(
            search, city, isActive, sortBy, isDescending, pageNumber, pageSize, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Gets supplier details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SupplierResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SupplierResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSupplierById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _supplierService.GetSupplierByIdAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Creates a new supplier.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SupplierResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SupplierResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierRequest request, CancellationToken cancellationToken)
    {
        var response = await _supplierService.CreateSupplierAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Updates an existing supplier.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SupplierResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SupplierResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateSupplier(Guid id, [FromBody] UpdateSupplierRequest request, CancellationToken cancellationToken)
    {
        var response = await _supplierService.UpdateSupplierAsync(id, request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Soft deletes a supplier.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteSupplier(Guid id, CancellationToken cancellationToken)
    {
        var response = await _supplierService.DeleteSupplierAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Gets supplier balance.
    /// </summary>
    [HttpGet("{id:guid}/balance")]
    [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSupplierBalance(Guid id, CancellationToken cancellationToken)
    {
        var response = await _supplierService.GetSupplierBalanceAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Gets supplier purchase history.
    /// </summary>
    [HttpGet("{id:guid}/purchases")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SupplierPurchaseHistoryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SupplierPurchaseHistoryResponse>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSupplierPurchases(Guid id, CancellationToken cancellationToken)
    {
        var response = await _supplierService.GetSupplierPurchasesAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
