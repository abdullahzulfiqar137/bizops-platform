using BusinessOperations.API.DTOs.Audit;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface IAuditLogService
{
    Task<PagedResponse<AuditLogResponse>> GetAuditLogsAsync(
        string? entityName,
        string? entityId,
        Guid? userId,
        string? action,
        DateTime? startDate,
        DateTime? endDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<AuditLogResponse>> GetAuditLogByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
