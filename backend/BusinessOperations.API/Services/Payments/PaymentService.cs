using BusinessOperations.API.Data;
using BusinessOperations.API.DTOs.Payments;
using BusinessOperations.API.Entities.Payments;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Repositories;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Services.Payments;

public sealed class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        ApplicationDbContext context,
        IPaymentRepository paymentRepository,
        ICustomerRepository customerRepository,
        ISupplierRepository supplierRepository,
        ISaleRepository saleRepository,
        IPurchaseRepository purchaseRepository,
        ICurrentUserService currentUserService,
        ILogger<PaymentService> logger)
    {
        _context = context;
        _paymentRepository = paymentRepository;
        _customerRepository = customerRepository;
        _supplierRepository = supplierRepository;
        _saleRepository = saleRepository;
        _purchaseRepository = purchaseRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<ApiResponse<PaymentResponse>> ProcessPaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        if (request.PaymentType == PaymentType.CustomerPayment)
        {
            if (!request.CustomerId.HasValue)
            {
                return ApiResponse<PaymentResponse>.FailureResponse("Customer ID is required.");
            }

            var customer = await _customerRepository.GetByIdAsync(request.CustomerId.Value, cancellationToken);
            if (customer == null)
            {
                return ApiResponse<PaymentResponse>.FailureResponse("Customer not found.");
            }

            if (request.SaleId.HasValue)
            {
                var sale = await _saleRepository.GetByIdAsync(request.SaleId.Value, cancellationToken);
                if (sale == null)
                {
                    return ApiResponse<PaymentResponse>.FailureResponse("Sale invoice not found.");
                }

                if (sale.CustomerId != customer.Id)
                {
                    return ApiResponse<PaymentResponse>.FailureResponse("The selected invoice does not belong to this customer.");
                }

                if (request.Amount > sale.DueAmount)
                {
                    return ApiResponse<PaymentResponse>.FailureResponse($"Payment exceeds the invoice balance of {sale.DueAmount:0.00}.");
                }

                sale.PaidAmount += request.Amount;
                sale.DueAmount = sale.TotalAmount - sale.PaidAmount;
                sale.DueAmount = sale.DueAmount < 0 ? 0 : sale.DueAmount;

                if (sale.PaidAmount >= sale.TotalAmount)
                {
                    sale.PaymentStatus = PaymentStatus.Paid;
                }
                else if (sale.PaidAmount > 0)
                {
                    sale.PaymentStatus = PaymentStatus.Partial;
                }

                sale.UpdatedBy = _currentUserService.Username;
                sale.UpdatedAtUtc = DateTime.UtcNow;
                await _saleRepository.UpdateAsync(sale, cancellationToken);
            }
            else if (request.Amount > customer.CurrentBalance)
            {
                return ApiResponse<PaymentResponse>.FailureResponse($"Payment exceeds the customer balance of {customer.CurrentBalance:0.00}.");
            }

            // Deduct customer balance
            customer.CurrentBalance -= request.Amount;
            customer.UpdatedBy = _currentUserService.Username;
            customer.UpdatedAtUtc = DateTime.UtcNow;
            await _customerRepository.UpdateAsync(customer, cancellationToken);
        }
        else if (request.PaymentType == PaymentType.SupplierPayment)
        {
            if (!request.SupplierId.HasValue)
            {
                return ApiResponse<PaymentResponse>.FailureResponse("Supplier ID is required.");
            }

            var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId.Value, cancellationToken);
            if (supplier == null)
            {
                return ApiResponse<PaymentResponse>.FailureResponse("Supplier not found.");
            }

            if (request.PurchaseId.HasValue)
            {
                var purchase = await _purchaseRepository.GetByIdAsync(request.PurchaseId.Value, cancellationToken);
                if (purchase == null)
                {
                    return ApiResponse<PaymentResponse>.FailureResponse("Purchase order not found.");
                }

                if (purchase.SupplierId != supplier.Id)
                {
                    return ApiResponse<PaymentResponse>.FailureResponse("The selected purchase does not belong to this supplier.");
                }

                if (request.Amount > purchase.DueAmount)
                {
                    return ApiResponse<PaymentResponse>.FailureResponse($"Payment exceeds the purchase balance of {purchase.DueAmount:0.00}.");
                }

                purchase.PaidAmount += request.Amount;
                purchase.DueAmount = purchase.TotalAmount - purchase.PaidAmount;
                purchase.DueAmount = purchase.DueAmount < 0 ? 0 : purchase.DueAmount;

                if (purchase.PaidAmount >= purchase.TotalAmount)
                {
                    purchase.PaymentStatus = PaymentStatus.Paid;
                }
                else if (purchase.PaidAmount > 0)
                {
                    purchase.PaymentStatus = PaymentStatus.Partial;
                }

                purchase.UpdatedBy = _currentUserService.Username;
                purchase.UpdatedAtUtc = DateTime.UtcNow;
                await _purchaseRepository.UpdateAsync(purchase, cancellationToken);
            }
            else if (request.Amount > supplier.CurrentBalance)
            {
                return ApiResponse<PaymentResponse>.FailureResponse($"Payment exceeds the supplier balance of {supplier.CurrentBalance:0.00}.");
            }

            // Deduct supplier balance
            supplier.CurrentBalance -= request.Amount;
            supplier.UpdatedBy = _currentUserService.Username;
            supplier.UpdatedAtUtc = DateTime.UtcNow;
            await _supplierRepository.UpdateAsync(supplier, cancellationToken);
        }

        var paymentNumber = await _paymentRepository.GenerateNextPaymentNumberAsync(cancellationToken);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            PaymentNumber = paymentNumber,
            PaymentType = request.PaymentType,
            CustomerId = request.CustomerId,
            SupplierId = request.SupplierId,
            SaleId = request.SaleId,
            PurchaseId = request.PurchaseId,
            Amount = request.Amount,
            PaymentDateUtc = DateTime.UtcNow,
            PaymentMethod = request.PaymentMethod,
            ReferenceNumber = request.ReferenceNumber,
            Notes = request.Notes,
            CreatedBy = _currentUserService.Username ?? "System",
            CreatedAtUtc = DateTime.UtcNow
        };

        await _paymentRepository.AddAsync(payment, cancellationToken);
        _logger.LogInformation("Payment processed successfully. PaymentNo: {PaymentNo}, Type: {Type}, Amount: {Amount}",
            payment.PaymentNumber, payment.PaymentType, payment.Amount);

        var createdPayment = await _paymentRepository.GetPaymentWithDetailsAsync(payment.Id, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ApiResponse<PaymentResponse>.SuccessResponse(MapToResponse(createdPayment!), "Payment processed successfully.");
    }

    public async Task<PagedResponse<PaymentResponse>> GetPaymentsAsync(
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
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

        var (items, totalCount) = await _paymentRepository.GetPagedPaymentsAsync(
            paymentType, customerId, supplierId, saleId, purchaseId, startDate, endDate, pageNumber, pageSize, cancellationToken);

        var responses = items.Select(MapToResponse);

        return new PagedResponse<PaymentResponse>(responses, totalCount, pageNumber, pageSize, "Payments retrieved successfully.");
    }

    public async Task<ApiResponse<PaymentResponse>> GetPaymentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetPaymentWithDetailsAsync(id, cancellationToken);
        if (payment == null)
        {
            return ApiResponse<PaymentResponse>.FailureResponse("Payment record not found.");
        }

        return ApiResponse<PaymentResponse>.SuccessResponse(MapToResponse(payment), "Payment details retrieved successfully.");
    }

    private static PaymentResponse MapToResponse(Payment payment)
    {
        return new PaymentResponse
        {
            Id = payment.Id,
            PaymentNumber = payment.PaymentNumber,
            PaymentType = payment.PaymentType,
            CustomerId = payment.CustomerId,
            CustomerName = payment.Customer?.Name,
            SupplierId = payment.SupplierId,
            SupplierName = payment.Supplier?.Name,
            SaleId = payment.SaleId,
            InvoiceNumber = payment.Sale?.InvoiceNumber,
            PurchaseId = payment.PurchaseId,
            PurchaseOrderNumber = payment.Purchase?.PurchaseOrderNumber,
            Amount = payment.Amount,
            PaymentDateUtc = payment.PaymentDateUtc,
            PaymentMethod = payment.PaymentMethod,
            ReferenceNumber = payment.ReferenceNumber,
            Notes = payment.Notes,
            CreatedBy = payment.CreatedBy ?? string.Empty,
            CreatedAtUtc = payment.CreatedAtUtc
        };
    }
}
