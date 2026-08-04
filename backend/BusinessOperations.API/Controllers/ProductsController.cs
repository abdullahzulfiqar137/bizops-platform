using BusinessOperations.API.DTOs.Products;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IProductMediaService _productMedia;

    public ProductsController(IProductService productService, IProductMediaService productMedia)
    {
        _productService = productService;
        _productMedia = productMedia;
    }

    [HttpPost("{id:guid}/image")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile file, CancellationToken token)
    {
        var response = await _productMedia.UploadImageAsync(id, file, token);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{id:guid}/image")]
    public async Task<IActionResult> RemoveImage(Guid id, CancellationToken token)
    {
        var response = await _productMedia.RemoveImageAsync(id, token);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Gets product list with search, category filtering, stock alerts, sorting, and pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? search,
        [FromQuery] Guid? categoryId,
        [FromQuery] bool? isActive,
        [FromQuery] bool? isLowStock,
        [FromQuery] string? sortBy,
        [FromQuery] bool isDescending = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var response = await _productService.GetProductsAsync(
            search, categoryId, isActive, isLowStock, sortBy, isDescending, pageNumber, pageSize, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Gets available product measurement units.
    /// </summary>
    [HttpGet("units")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductUnitResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductUnits(CancellationToken cancellationToken)
    {
        var response = await _productService.GetProductUnitsAsync(cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Gets product details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _productService.GetProductByIdAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var response = await _productService.CreateProductAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var response = await _productService.UpdateProductAsync(id, request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Soft deletes a product.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        var response = await _productService.DeleteProductAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
