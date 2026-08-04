using BusinessOperations.API.Entities.Audit;

namespace BusinessOperations.API.Interfaces.Repositories;

public interface IAuditLogRepository : IGenericRepository<AuditLog>
{
    Task<(IEnumerable<AuditLog> Items, int TotalCount)> GetPagedAuditLogsAsync(
        string? entityName,
        string? entityId,
        Guid? userId,
        string? action,
        DateTime? startDate,
        DateTime? endDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
