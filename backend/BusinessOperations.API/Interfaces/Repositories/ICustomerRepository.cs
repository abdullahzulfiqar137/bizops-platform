using BusinessOperations.API.Entities.Customers;

namespace BusinessOperations.API.Interfaces.Repositories;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<(IEnumerable<Customer> Items, int TotalCount)> GetPagedCustomersAsync(
        string? searchTerm,
        string? city,
        bool? isActive,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Customer?> GetByCodeAsync(string customerCode, CancellationToken cancellationToken = default);
    Task<string> GenerateNextCustomerCodeAsync(CancellationToken cancellationToken = default);
}
