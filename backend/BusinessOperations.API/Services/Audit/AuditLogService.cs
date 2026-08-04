using BusinessOperations.API.DTOs.Audit;
using BusinessOperations.API.Entities.Audit;
using BusinessOperations.API.Interfaces.Repositories;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Services.Audit;

public sealed class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<PagedResponse<AuditLogResponse>> GetAuditLogsAsync(
        string? entityName,
        string? entityId,
        Guid? userId,
        string? action,
        DateTime? startDate,
        DateTime? endDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

        var (items, totalCount) = await _auditLogRepository.GetPagedAuditLogsAsync(
            entityName, entityId, userId, action, startDate, endDate, pageNumber, pageSize, cancellationToken);

        var responses = items.Select(MapToResponse);

        return new PagedResponse<AuditLogResponse>(responses, totalCount, pageNumber, pageSize, "Audit logs retrieved successfully.");
    }

    public async Task<ApiResponse<AuditLogResponse>> GetAuditLogByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var auditLog = await _auditLogRepository.GetByIdAsync(id, cancellationToken);
        if (auditLog == null)
        {
            return ApiResponse<AuditLogResponse>.FailureResponse("Audit log record not found.");
        }

        return ApiResponse<AuditLogResponse>.SuccessResponse(MapToResponse(auditLog), "Audit log details retrieved successfully.");
    }

    private static AuditLogResponse MapToResponse(AuditLog auditLog)
    {
        return new AuditLogResponse
        {
            Id = auditLog.Id,
            UserId = auditLog.UserId,
            UserName = auditLog.UserName,
            Action = auditLog.Action,
            EntityName = auditLog.EntityName,
            EntityId = auditLog.EntityId,
            OldValues = auditLog.OldValues,
            NewValues = auditLog.NewValues,
            AffectedColumns = auditLog.AffectedColumns,
            IpAddress = auditLog.IpAddress,
            UserAgent = auditLog.UserAgent,
            TimestampUtc = auditLog.TimestampUtc
        };
    }
}
