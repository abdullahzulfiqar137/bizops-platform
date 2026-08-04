using BusinessOperations.API.DTOs.Backups;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/database-backups")]
[Authorize(Roles = "Admin")]
public sealed class DatabaseBackupsController : ControllerBase
{
    private readonly IDatabaseBackupService _backupService;

    public DatabaseBackupsController(IDatabaseBackupService backupService)
    {
        _backupService = backupService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DatabaseBackupResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken cancellationToken) =>
        Ok(await _backupService.ListAsync(cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DatabaseBackupResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        Ok(await _backupService.CreateAsync(cancellationToken));

    [HttpGet("{fileName}/download")]
    public IActionResult Download(string fileName)
    {
        var path = _backupService.ResolveBackupPath(fileName);
        if (path == null || !System.IO.File.Exists(path)) return NotFound();
        return PhysicalFile(path, "application/octet-stream", Path.GetFileName(path));
    }

    [HttpPost("restore")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Restore(
        [FromBody] RestoreDatabaseRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _backupService.RestoreAsync(request, cancellationToken);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}
