namespace BusinessOperations.API.DTOs.Backups;

public sealed class DatabaseBackupResponse
{
    public string FileName { get; init; } = string.Empty;
    public long SizeBytes { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
