namespace BusinessOperations.API.Models;

public sealed class ValidationErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = "Validation failed.";
    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
}
