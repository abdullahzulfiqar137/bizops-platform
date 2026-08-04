namespace BusinessOperations.API.Models;

public sealed class HealthCheckResponse
{
    public string Application { get; set; } = "Business Operations API";
    public string Status { get; set; } = "Healthy";
    public string Version { get; set; } = "1.0.0";
    public DateTime UtcTimestamp { get; set; } = DateTime.UtcNow;
    public bool DatabaseAvailable { get; set; }
    public long UptimeSeconds { get; set; }
    public long AvailableStorageBytes { get; set; }
    public DateTime? LastBackupAtUtc { get; set; }
    public string BackupStatus { get; set; } = "Not configured";
}
