using BusinessOperations.API.DTOs.Customers;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Gets customer list with search, filtering, sorting, and pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CustomerResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomers(
        [FromQuery] string? search,
        [FromQuery] string? city,
        [FromQuery] bool? isActive,
        [FromQuery] string? sortBy,
        [FromQuery] bool isDescending = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var response = await _customerService.GetCustomersAsync(
            search, city, isActive, sortBy, isDescending, pageNumber, pageSize, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Gets customer details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCustomerById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _customerService.GetCustomerByIdAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var response = await _customerService.CreateCustomerAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var response = await _customerService.UpdateCustomerAsync(id, request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Soft deletes a customer.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCustomer(Guid id, CancellationToken cancellationToken)
    {
        var response = await _customerService.DeleteCustomerAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Gets customer balance.
    /// </summary>
    [HttpGet("{id:guid}/balance")]
    [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCustomerBalance(Guid id, CancellationToken cancellationToken)
    {
        var response = await _customerService.GetCustomerBalanceAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Gets customer transaction history.
    /// </summary>
    [HttpGet("{id:guid}/transactions")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerTransactionHistoryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerTransactionHistoryResponse>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCustomerTransactions(Guid id, CancellationToken cancellationToken)
    {
        var response = await _customerService.GetCustomerTransactionsAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
