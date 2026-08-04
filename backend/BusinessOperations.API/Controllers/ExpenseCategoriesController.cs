using BusinessOperations.API.DTOs.Expenses;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/expense-categories")]
[Authorize]
public sealed class ExpenseCategoriesController : ControllerBase
{
    private readonly IExpenseCategoryService _categoryService;

    public ExpenseCategoriesController(IExpenseCategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Gets all active expense categories.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ExpenseCategoryResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCategories(CancellationToken cancellationToken)
    {
        var response = await _categoryService.GetAllCategoriesAsync(cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Gets expense category details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ExpenseCategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ExpenseCategoryResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _categoryService.GetCategoryByIdAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Creates a new expense category.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ExpenseCategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ExpenseCategoryResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateExpenseCategoryRequest request, CancellationToken cancellationToken)
    {
        var response = await _categoryService.CreateCategoryAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Updates an existing expense category.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ExpenseCategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ExpenseCategoryResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateExpenseCategoryRequest request, CancellationToken cancellationToken)
    {
        var response = await _categoryService.UpdateCategoryAsync(id, request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Soft-deletes an expense category.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        var response = await _categoryService.DeleteCategoryAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
