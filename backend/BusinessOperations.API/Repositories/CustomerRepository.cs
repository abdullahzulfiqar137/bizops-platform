using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Customers;
using BusinessOperations.API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Repositories;

public sealed class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Customer> Items, int TotalCount)> GetPagedCustomersAsync(
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
            query = query.Where(c =>
                c.Name.ToLower().Contains(search) ||
                c.CustomerCode.ToLower().Contains(search) ||
                c.Email.ToLower().Contains(search) ||
                c.PhoneNumber.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(c => c.City != null && c.City.ToLower() == city.Trim().ToLower());
        }

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        query = sortBy?.ToLower() switch
        {
            "name" => isDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
            "customercode" => isDescending ? query.OrderByDescending(c => c.CustomerCode) : query.OrderBy(c => c.CustomerCode),
            "balance" => isDescending ? query.OrderByDescending(c => c.CurrentBalance) : query.OrderBy(c => c.CurrentBalance),
            _ => isDescending ? query.OrderByDescending(c => c.CreatedAtUtc) : query.OrderBy(c => c.CreatedAtUtc)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Customer?> GetByCodeAsync(string customerCode, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.CustomerCode == customerCode, cancellationToken);
    }

    public async Task<string> GenerateNextCustomerCodeAsync(CancellationToken cancellationToken = default)
    {
        var count = await DbSet.IgnoreQueryFilters()
            .CountAsync(x => x.TenantId == Context.CurrentTenantId, cancellationToken);
        return $"CUST-{(count + 1):D5}";
    }
}
