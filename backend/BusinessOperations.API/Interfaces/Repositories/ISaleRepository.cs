using BusinessOperations.API.Entities.Sales;
using BusinessOperations.API.Enums;

namespace BusinessOperations.API.Interfaces.Repositories;

public interface ISaleRepository : IGenericRepository<Sale>
{
    Task<(IEnumerable<Sale> Items, int TotalCount)> GetPagedSalesAsync(
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
        CancellationToken cancellationToken = default);

    Task<Sale?> GetSaleWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<string> GenerateNextInvoiceNumberAsync(CancellationToken cancellationToken = default);
}
