namespace BusinessOperations.API.Configurations;

public sealed class BackupSettings
{
    public const string SectionName = "DatabaseBackups";

    public string Directory { get; set; } = "Backups";
    public bool EnableRestore { get; set; }
    public int RetentionDays { get; set; } = 30;
    public bool ScheduleEnabled { get; set; }
    public int ScheduleIntervalHours { get; set; } = 24;
    public string? OffsiteDirectory { get; set; }
}
