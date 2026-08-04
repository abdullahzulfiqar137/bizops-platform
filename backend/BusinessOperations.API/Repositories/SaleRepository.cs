using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Sales;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Repositories;

public sealed class SaleRepository : GenericRepository<Sale>, ISaleRepository
{
    public SaleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Sale> Items, int TotalCount)> GetPagedSalesAsync(
        string? searchTerm,
        Guid? customerId,
        SaleStatus? status,
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
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
                .ThenInclude(i => i.Product)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim().ToLower();
            query = query.Where(s =>
                s.InvoiceNumber.ToLower().Contains(search) ||
                s.Customer.Name.ToLower().Contains(search));
        }

        if (customerId.HasValue)
        {
            query = query.Where(s => s.CustomerId == customerId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(s => s.Status == status.Value);
        }

        if (paymentStatus.HasValue)
        {
            query = query.Where(s => s.PaymentStatus == paymentStatus.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(s => s.SaleDateUtc >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(s => s.SaleDateUtc <= endDate.Value);
        }

        query = sortBy?.ToLower() switch
        {
            "invoicenumber" => isDescending ? query.OrderByDescending(s => s.InvoiceNumber) : query.OrderBy(s => s.InvoiceNumber),
            "totalamount" => isDescending ? query.OrderByDescending(s => s.TotalAmount) : query.OrderBy(s => s.TotalAmount),
            "saledate" => isDescending ? query.OrderByDescending(s => s.SaleDateUtc) : query.OrderBy(s => s.SaleDateUtc),
            _ => isDescending ? query.OrderByDescending(s => s.CreatedAtUtc) : query.OrderBy(s => s.CreatedAtUtc)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Sale?> GetSaleWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<string> GenerateNextInvoiceNumberAsync(CancellationToken cancellationToken = default)
    {
        var count = await DbSet.IgnoreQueryFilters()
            .CountAsync(x => x.TenantId == Context.CurrentTenantId, cancellationToken);
        return $"INV-{(count + 1001):D6}";
    }
}
