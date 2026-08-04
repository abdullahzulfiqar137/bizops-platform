using BusinessOperations.API.DTOs.Expenses;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    /// <summary>
    /// Creates a new operational expense entry.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ExpenseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ExpenseResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseRequest request, CancellationToken cancellationToken)
    {
        var response = await _expenseService.CreateExpenseAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Gets expenses list with search, filtering by category/date, sorting, and pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ExpenseResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExpenses(
        [FromQuery] string? search,
        [FromQuery] Guid? expenseCategoryId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? sortBy,
        [FromQuery] bool isDescending = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var response = await _expenseService.GetExpensesAsync(
            search, expenseCategoryId, startDate, endDate, sortBy, isDescending, pageNumber, pageSize, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Gets expense details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ExpenseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ExpenseResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetExpenseById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _expenseService.GetExpenseByIdAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Updates an existing expense entry.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ExpenseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ExpenseResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateExpense(Guid id, [FromBody] UpdateExpenseRequest request, CancellationToken cancellationToken)
    {
        var response = await _expenseService.UpdateExpenseAsync(id, request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Soft-deletes an expense entry.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteExpense(Guid id, CancellationToken cancellationToken)
    {
        var response = await _expenseService.DeleteExpenseAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
