namespace BusinessOperations.API.DTOs.Expenses;

public sealed class CreateExpenseCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
