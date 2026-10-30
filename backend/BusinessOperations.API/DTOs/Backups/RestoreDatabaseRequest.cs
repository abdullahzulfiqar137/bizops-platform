namespace BusinessOperations.API.DTOs.Backups;

public sealed class RestoreDatabaseRequest
{
    public string FileName { get; init; } = string.Empty;
    public string Confirmation { get; init; } = string.Empty;
}
