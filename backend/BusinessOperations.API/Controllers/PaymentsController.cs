using BusinessOperations.API.DTOs.Payments;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Processes a customer or supplier payment. Updates invoice/PO balances and entity ledger.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessPayment([FromBody] CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var response = await _paymentService.ProcessPaymentAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Gets payment history with filtering by type, customer, supplier, invoice, PO, or date range.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<PaymentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPayments(
        [FromQuery] PaymentType? paymentType,
        [FromQuery] Guid? customerId,
        [FromQuery] Guid? supplierId,
        [FromQuery] Guid? saleId,
        [FromQuery] Guid? purchaseId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var response = await _paymentService.GetPaymentsAsync(
            paymentType, customerId, supplierId, saleId, purchaseId, startDate, endDate, pageNumber, pageSize, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Gets payment details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPaymentById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _paymentService.GetPaymentByIdAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
