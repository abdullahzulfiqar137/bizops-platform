using BusinessOperations.API.Entities.Suppliers;

namespace BusinessOperations.API.Interfaces.Repositories;

public interface ISupplierRepository : IGenericRepository<Supplier>
{
    Task<(IEnumerable<Supplier> Items, int TotalCount)> GetPagedSuppliersAsync(
        string? searchTerm,
        string? city,
        bool? isActive,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Supplier?> GetByCodeAsync(string supplierCode, CancellationToken cancellationToken = default);
    Task<string> GenerateNextSupplierCodeAsync(CancellationToken cancellationToken = default);
}
