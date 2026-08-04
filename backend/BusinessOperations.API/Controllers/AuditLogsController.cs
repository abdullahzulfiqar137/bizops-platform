using BusinessOperations.API.DTOs.Audit;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/audit-logs")]
[Authorize(Roles = "Admin,Manager")]
public sealed class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Gets system audit logs with filtering by EntityName, EntityId, UserId, Action, and DateRange.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<AuditLogResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] string? entityName,
        [FromQuery] string? entityId,
        [FromQuery] Guid? userId,
        [FromQuery] string? action,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var response = await _auditLogService.GetAuditLogsAsync(
            entityName, entityId, userId, action, startDate, endDate, pageNumber, pageSize, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Gets audit log details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AuditLogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuditLogResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAuditLogById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _auditLogService.GetAuditLogByIdAsync(id, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
