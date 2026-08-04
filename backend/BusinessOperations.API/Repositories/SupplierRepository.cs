using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Suppliers;
using BusinessOperations.API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Repositories;

public sealed class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
{
    public SupplierRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Supplier> Items, int TotalCount)> GetPagedSuppliersAsync(
        string? searchTerm,
        string? city,
        bool? isActive,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim().ToLower();
            query = query.Where(s =>
                s.Name.ToLower().Contains(search) ||
                s.CompanyName.ToLower().Contains(search) ||
                s.SupplierCode.ToLower().Contains(search) ||
                s.Email.ToLower().Contains(search) ||
                s.PhoneNumber.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(s => s.City != null && s.City.ToLower() == city.Trim().ToLower());
        }

        if (isActive.HasValue)
        {
            query = query.Where(s => s.IsActive == isActive.Value);
        }

        query = sortBy?.ToLower() switch
        {
            "name" => isDescending ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
            "companyname" => isDescending ? query.OrderByDescending(s => s.CompanyName) : query.OrderBy(s => s.CompanyName),
            "suppliercode" => isDescending ? query.OrderByDescending(s => s.SupplierCode) : query.OrderBy(s => s.SupplierCode),
            "balance" => isDescending ? query.OrderByDescending(s => s.CurrentBalance) : query.OrderBy(s => s.CurrentBalance),
            _ => isDescending ? query.OrderByDescending(s => s.CreatedAtUtc) : query.OrderBy(s => s.CreatedAtUtc)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Supplier?> GetByCodeAsync(string supplierCode, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(s => s.SupplierCode == supplierCode, cancellationToken);
    }

    public async Task<string> GenerateNextSupplierCodeAsync(CancellationToken cancellationToken = default)
    {
        var count = await DbSet.IgnoreQueryFilters()
            .CountAsync(x => x.TenantId == Context.CurrentTenantId, cancellationToken);
        return $"SUPP-{(count + 1):D5}";
    }
}
