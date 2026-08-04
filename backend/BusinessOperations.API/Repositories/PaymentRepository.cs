using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Payments;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Repositories;

public sealed class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Payment> Items, int TotalCount)> GetPagedPaymentsAsync(
        PaymentType? paymentType,
        Guid? customerId,
        Guid? supplierId,
        Guid? saleId,
        Guid? purchaseId,
        DateTime? startDate,
        DateTime? endDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(p => p.Customer)
            .Include(p => p.Supplier)
            .Include(p => p.Sale)
            .Include(p => p.Purchase)
            .AsNoTracking();

        if (paymentType.HasValue)
        {
            query = query.Where(p => p.PaymentType == paymentType.Value);
        }

        if (customerId.HasValue)
        {
            query = query.Where(p => p.CustomerId == customerId.Value);
        }

        if (supplierId.HasValue)
        {
            query = query.Where(p => p.SupplierId == supplierId.Value);
        }

        if (saleId.HasValue)
        {
            query = query.Where(p => p.SaleId == saleId.Value);
        }

        if (purchaseId.HasValue)
        {
            query = query.Where(p => p.PurchaseId == purchaseId.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(p => p.PaymentDateUtc >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(p => p.PaymentDateUtc <= endDate.Value);
        }

        query = query.OrderByDescending(p => p.CreatedAtUtc);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Payment?> GetPaymentWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Customer)
            .Include(p => p.Supplier)
            .Include(p => p.Sale)
            .Include(p => p.Purchase)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<string> GenerateNextPaymentNumberAsync(CancellationToken cancellationToken = default)
    {
        var count = await DbSet.IgnoreQueryFilters()
            .CountAsync(x => x.TenantId == Context.CurrentTenantId, cancellationToken);
        return $"PAY-{(count + 1001):D6}";
    }
}
