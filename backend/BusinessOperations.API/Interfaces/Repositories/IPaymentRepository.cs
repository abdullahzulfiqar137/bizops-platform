using BusinessOperations.API.Entities.Payments;
using BusinessOperations.API.Enums;

namespace BusinessOperations.API.Interfaces.Repositories;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<(IEnumerable<Payment> Items, int TotalCount)> GetPagedPaymentsAsync(
        PaymentType? paymentType,
        Guid? customerId,
        Guid? supplierId,
        Guid? saleId,
        Guid? purchaseId,
        DateTime? startDate,
        DateTime? endDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Payment?> GetPaymentWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<string> GenerateNextPaymentNumberAsync(CancellationToken cancellationToken = default);
}
