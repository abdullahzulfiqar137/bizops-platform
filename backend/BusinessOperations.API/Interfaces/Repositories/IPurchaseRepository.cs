using BusinessOperations.API.Entities.Purchases;
using BusinessOperations.API.Enums;

namespace BusinessOperations.API.Interfaces.Repositories;

public interface IPurchaseRepository : IGenericRepository<Purchase>
{
    Task<(IEnumerable<Purchase> Items, int TotalCount)> GetPagedPurchasesAsync(
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
        CancellationToken cancellationToken = default);

    Task<Purchase?> GetPurchaseWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<string> GenerateNextPurchaseOrderNumberAsync(CancellationToken cancellationToken = default);
}
