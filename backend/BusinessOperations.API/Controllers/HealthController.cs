using Microsoft.AspNetCore.Mvc;
using BusinessOperations.API.Models;
using BusinessOperations.API.Data;
using BusinessOperations.API.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    private static readonly DateTime StartedAtUtc = DateTime.UtcNow;
    private readonly ApplicationDbContext _db;
    private readonly IDatabaseBackupService _backups;
    private readonly IWebHostEnvironment _environment;
    public HealthController(ApplicationDbContext db, IDatabaseBackupService backups, IWebHostEnvironment environment)
    {
        _db = db; _backups = backups; _environment = environment;
    }
    /// <summary>
    /// Checks the API health status.
    /// </summary>
    /// <returns>Returns application status and current UTC timestamp.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<HealthCheckResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHealthStatus(CancellationToken token)
    {
        var databaseAvailable = await _db.Database.CanConnectAsync(token);
        var backupResult = await _backups.ListAsync(token);
        var lastBackup = backupResult.Data?.FirstOrDefault()?.CreatedAtUtc;
        var driveRoot = Path.GetPathRoot(_environment.ContentRootPath)!;
        var availableStorage = new DriveInfo(driveRoot).AvailableFreeSpace;
        var backupHealthy = lastBackup.HasValue && lastBackup.Value >= DateTime.UtcNow.AddHours(-48);
        var responseData = new HealthCheckResponse
        {
            Application = "Business Operations API",
            Version = "1.0.0",
            UtcTimestamp = DateTime.UtcNow,
            DatabaseAvailable = databaseAvailable,
            UptimeSeconds = (long)(DateTime.UtcNow - StartedAtUtc).TotalSeconds,
            AvailableStorageBytes = availableStorage,
            LastBackupAtUtc = lastBackup,
            BackupStatus = backupHealthy ? "Recent" : lastBackup.HasValue ? "Stale" : "No backup found",
            Status = databaseAvailable ? "Healthy" : "Unhealthy"
        };

        var response = ApiResponse<HealthCheckResponse>.SuccessResponse(
            responseData,
            "Business Operations API is running."
        );

        return databaseAvailable ? Ok(response) : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
    }
}
