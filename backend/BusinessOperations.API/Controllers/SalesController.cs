using BusinessOperations.API.DTOs.Sales;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using BusinessOperations.API.Services.Pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;
    private readonly IPdfDocumentService _pdfDocumentService;

    public SalesController(ISaleService saleService, IPdfDocumentService pdfDocumentService)
    {
        _saleService = saleService;
        _pdfDocumentService = pdfDocumentService;
    }

    /// <summary>
    /// Creates a new sales invoice. Automatically updates product stock and customer balance.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SaleResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var response = await _saleService.CreateSaleAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Gets sales invoice list with search, filtering by customer/status/date, sorting, and pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<SaleResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSales(
        [FromQuery] string? search,
        [FromQuery] Guid? customerId,
        [FromQuery] SaleStatus? status,
        [FromQuery] PaymentStatus? paymentStatus,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? sortBy,
        [FromQuery] bool isDescending = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var response = await _saleService.GetSalesAsync(
            search, customerId, status, paymentStatus, startDate, endDate, sortBy, isDescending, pageNumber, pageSize, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Gets sales invoice details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SaleResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSaleById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _saleService.GetSaleByIdAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Downloads or previews official computer-generated PDF tax invoice for a sale.
    /// </summary>
    [HttpGet("{id:guid}/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSaleInvoicePdf(Guid id, CancellationToken cancellationToken)
    {
        var sale = await _saleService.GetSaleEntityByIdAsync(id, cancellationToken);
        if (sale == null)
        {
            return NotFound(new { Message = "Sale invoice not found." });
        }

        var pdfBytes = _pdfDocumentService.GenerateSaleInvoicePdf(sale);
        var fileName = $"Invoice_{sale.InvoiceNumber}.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }

    /// <summary>
    /// Cancels a sales invoice, reverting product stock and customer balance.
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelSale(Guid id, CancellationToken cancellationToken)
    {
        var response = await _saleService.CancelSaleAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
