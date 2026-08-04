using BusinessOperations.API.DTOs.Payments;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface IPaymentService
{
    Task<ApiResponse<PaymentResponse>> ProcessPaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default);

    Task<PagedResponse<PaymentResponse>> GetPaymentsAsync(
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

    Task<ApiResponse<PaymentResponse>> GetPaymentByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
