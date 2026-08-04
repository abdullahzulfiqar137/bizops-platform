using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Purchases;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Repositories;

public sealed class PurchaseRepository : GenericRepository<Purchase>, IPurchaseRepository
{
    public PurchaseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Purchase> Items, int TotalCount)> GetPagedPurchasesAsync(
        string? searchTerm,
        Guid? supplierId,
        PurchaseStatus? status,
        PaymentStatus? paymentStatus,
        DateTime? startDate,
        DateTime? endDate,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(p => p.Supplier)
            .Include(p => p.PurchaseItems)
                .ThenInclude(i => i.Product)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim().ToLower();
            query = query.Where(p =>
                p.PurchaseOrderNumber.ToLower().Contains(search) ||
                p.Supplier.CompanyName.ToLower().Contains(search) ||
                p.Supplier.Name.ToLower().Contains(search));
        }

        if (supplierId.HasValue)
        {
            query = query.Where(p => p.SupplierId == supplierId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (paymentStatus.HasValue)
        {
            query = query.Where(p => p.PaymentStatus == paymentStatus.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(p => p.PurchaseDateUtc >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(p => p.PurchaseDateUtc <= endDate.Value);
        }

        query = sortBy?.ToLower() switch
        {
            "ponumber" => isDescending ? query.OrderByDescending(p => p.PurchaseOrderNumber) : query.OrderBy(p => p.PurchaseOrderNumber),
            "totalamount" => isDescending ? query.OrderByDescending(p => p.TotalAmount) : query.OrderBy(p => p.TotalAmount),
            "purchasedate" => isDescending ? query.OrderByDescending(p => p.PurchaseDateUtc) : query.OrderBy(p => p.PurchaseDateUtc),
            _ => isDescending ? query.OrderByDescending(p => p.CreatedAtUtc) : query.OrderBy(p => p.CreatedAtUtc)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Purchase?> GetPurchaseWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Supplier)
            .Include(p => p.PurchaseItems)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<string> GenerateNextPurchaseOrderNumberAsync(CancellationToken cancellationToken = default)
    {
        var count = await DbSet.IgnoreQueryFilters()
            .CountAsync(x => x.TenantId == Context.CurrentTenantId, cancellationToken);
        return $"PO-{(count + 1001):D6}";
    }
}
