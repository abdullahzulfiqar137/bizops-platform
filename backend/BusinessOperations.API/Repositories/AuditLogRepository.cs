using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Audit;
using BusinessOperations.API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Repositories;

public sealed class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<AuditLog> Items, int TotalCount)> GetPagedAuditLogsAsync(
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
        var query = DbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(entityName))
        {
            query = query.Where(a => a.EntityName.ToLower() == entityName.Trim().ToLower());
        }

        if (!string.IsNullOrWhiteSpace(entityId))
        {
            query = query.Where(a => a.EntityId.Contains(entityId.Trim()));
        }

        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(a => a.Action.ToLower() == action.Trim().ToLower());
        }

        if (startDate.HasValue)
        {
            query = query.Where(a => a.TimestampUtc >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(a => a.TimestampUtc <= endDate.Value);
        }

        query = query.OrderByDescending(a => a.TimestampUtc);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
