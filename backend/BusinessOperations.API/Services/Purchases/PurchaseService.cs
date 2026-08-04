using BusinessOperations.API.Data;
using BusinessOperations.API.DTOs.Purchases;
using BusinessOperations.API.Entities.Inventory;
using BusinessOperations.API.Entities.Purchases;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Repositories;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Services.Purchases;

public sealed class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IProductRepository _productRepository;
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<PurchaseService> _logger;

    public PurchaseService(
        IPurchaseRepository purchaseRepository,
        ISupplierRepository supplierRepository,
        IProductRepository productRepository,
        IInventoryMovementRepository movementRepository,
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<PurchaseService> logger)
    {
        _purchaseRepository = purchaseRepository;
        _supplierRepository = supplierRepository;
        _productRepository = productRepository;
        _movementRepository = movementRepository;
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<ApiResponse<PurchaseResponse>> CreatePurchaseAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier == null)
        {
            return ApiResponse<PurchaseResponse>.FailureResponse("Supplier not found.");
        }

        var purchaseItems = new List<PurchaseItem>();
        decimal subTotal = 0.00m;

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, cancellationToken);

        foreach (var itemReq in request.Items)
        {
            if (!products.TryGetValue(itemReq.ProductId, out var product))
            {
                return ApiResponse<PurchaseResponse>.FailureResponse($"Product with ID '{itemReq.ProductId}' was not found.");
            }

            var itemTotalPrice = itemReq.Quantity * itemReq.UnitCost;
            subTotal += itemTotalPrice;

            purchaseItems.Add(new PurchaseItem
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Quantity = itemReq.Quantity,
                UnitCost = itemReq.UnitCost,
                TotalPrice = itemTotalPrice
            });
        }

        var taxAmount = Math.Round(subTotal * (request.TaxRate / 100.00m), 2);
        var totalAmount = (subTotal + taxAmount) - request.DiscountAmount;
        totalAmount = totalAmount < 0 ? 0 : totalAmount;

        var dueAmount = totalAmount - request.PaidAmount;
        dueAmount = dueAmount < 0 ? 0 : dueAmount;

        PaymentStatus paymentStatus;
        if (request.PaidAmount >= totalAmount)
        {
            paymentStatus = PaymentStatus.Paid;
        }
        else if (request.PaidAmount > 0)
        {
            paymentStatus = PaymentStatus.Partial;
        }
        else
        {
            paymentStatus = PaymentStatus.Unpaid;
        }

        var poNumber = await _purchaseRepository.GenerateNextPurchaseOrderNumberAsync(cancellationToken);

        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),
            PurchaseOrderNumber = poNumber,
            SupplierId = supplier.Id,
            PurchaseDateUtc = DateTime.UtcNow,
            SubTotal = subTotal,
            TaxRate = request.TaxRate,
            TaxAmount = taxAmount,
            DiscountAmount = request.DiscountAmount,
            TotalAmount = totalAmount,
            PaidAmount = request.PaidAmount,
            DueAmount = dueAmount,
            Status = PurchaseStatus.Completed,
            PaymentStatus = paymentStatus,
            Notes = request.Notes,
            CreatedBy = _currentUserService.Username ?? "System",
            CreatedAtUtc = DateTime.UtcNow,
            PurchaseItems = purchaseItems
        };

        // Increase product inventory stock & record movement audit
        foreach (var item in purchaseItems)
        {
            var product = products[item.ProductId];
            var previousStock = product.StockQuantity;
            var newStock = previousStock + item.Quantity;

            product.StockQuantity = newStock;
            // Optionally update cost price if purchase unit cost differs
            product.CostPrice = item.UnitCost;
            product.UpdatedBy = _currentUserService.Username;
            product.UpdatedAtUtc = DateTime.UtcNow;

            var movementNumber = await _movementRepository.GenerateNextMovementNumberAsync(cancellationToken);
            var movement = new InventoryMovement
            {
                Id = Guid.NewGuid(),
                MovementNumber = movementNumber,
                ProductId = product.Id,
                MovementType = InventoryMovementType.In,
                Quantity = item.Quantity,
                PreviousStock = previousStock,
                NewStock = newStock,
                ReferenceType = "Purchase",
                ReferenceId = purchase.Id,
                Notes = $"Purchase Order: {purchase.PurchaseOrderNumber}",
                CreatedBy = _currentUserService.Username ?? "System",
                CreatedAtUtc = DateTime.UtcNow
            };

            await _movementRepository.AddAsync(movement, cancellationToken);
            await _productRepository.UpdateAsync(product, cancellationToken);
        }

        // Update supplier balance if remaining due balance exists
        if (dueAmount > 0)
        {
            supplier.CurrentBalance += dueAmount;
            supplier.UpdatedBy = _currentUserService.Username;
            supplier.UpdatedAtUtc = DateTime.UtcNow;
            await _supplierRepository.UpdateAsync(supplier, cancellationToken);
        }

        await _purchaseRepository.AddAsync(purchase, cancellationToken);
        _logger.LogInformation("Purchase order created successfully. PONumber: {PONumber}, Total: {TotalAmount}", purchase.PurchaseOrderNumber, purchase.TotalAmount);

        var createdPurchase = await _purchaseRepository.GetPurchaseWithItemsAsync(purchase.Id, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ApiResponse<PurchaseResponse>.SuccessResponse(MapToResponse(createdPurchase!), "Purchase order created successfully.");
    }

    public async Task<PagedResponse<PurchaseResponse>> GetPurchasesAsync(
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
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

        var (items, totalCount) = await _purchaseRepository.GetPagedPurchasesAsync(
            searchTerm, supplierId, status, paymentStatus, startDate, endDate, sortBy, isDescending, pageNumber, pageSize, cancellationToken);

        var responses = items.Select(MapToResponse);

        return new PagedResponse<PurchaseResponse>(responses, totalCount, pageNumber, pageSize, "Purchase orders retrieved successfully.");
    }

    public async Task<ApiResponse<PurchaseResponse>> GetPurchaseByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var purchase = await _purchaseRepository.GetPurchaseWithItemsAsync(id, cancellationToken);
        if (purchase == null)
        {
            return ApiResponse<PurchaseResponse>.FailureResponse("Purchase order not found.");
        }

        return ApiResponse<PurchaseResponse>.SuccessResponse(MapToResponse(purchase), "Purchase order retrieved successfully.");
    }

    public async Task<ApiResponse<bool>> CancelPurchaseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var purchase = await _purchaseRepository.GetPurchaseWithItemsAsync(id, cancellationToken);
        if (purchase == null)
        {
            return ApiResponse<bool>.FailureResponse("Purchase order not found.");
        }

        if (purchase.Status != PurchaseStatus.Completed)
        {
            return ApiResponse<bool>.FailureResponse("Only a completed purchase with no returns can be cancelled.");
        }

        // Revert stock & record movement
        foreach (var item in purchase.PurchaseItems)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product != null)
            {
                var previousStock = product.StockQuantity;
                if (previousStock < item.Quantity)
                {
                    return ApiResponse<bool>.FailureResponse($"Cannot cancel purchase because product '{product.Name}' does not have enough stock to revert ({previousStock} available, {item.Quantity} required).");
                }

                var newStock = previousStock - item.Quantity;

                product.StockQuantity = newStock;
                product.UpdatedBy = _currentUserService.Username;
                product.UpdatedAtUtc = DateTime.UtcNow;

                var movementNumber = await _movementRepository.GenerateNextMovementNumberAsync(cancellationToken);
                var movement = new InventoryMovement
                {
                    Id = Guid.NewGuid(),
                    MovementNumber = movementNumber,
                    ProductId = product.Id,
                    MovementType = InventoryMovementType.Out,
                    Quantity = item.Quantity,
                    PreviousStock = previousStock,
                    NewStock = newStock,
                    ReferenceType = "PurchaseCancellation",
                    ReferenceId = purchase.Id,
                    Notes = $"Cancelled Purchase Order: {purchase.PurchaseOrderNumber}",
                    CreatedBy = _currentUserService.Username ?? "System",
                    CreatedAtUtc = DateTime.UtcNow
                };

                await _movementRepository.AddAsync(movement, cancellationToken);
                await _productRepository.UpdateAsync(product, cancellationToken);
            }
        }

        // Revert supplier balance if due amount was added
        if (purchase.DueAmount > 0)
        {
            var supplier = await _supplierRepository.GetByIdAsync(purchase.SupplierId, cancellationToken);
            if (supplier != null)
            {
                supplier.CurrentBalance -= purchase.DueAmount;
                supplier.UpdatedBy = _currentUserService.Username;
                supplier.UpdatedAtUtc = DateTime.UtcNow;
                await _supplierRepository.UpdateAsync(supplier, cancellationToken);
            }
        }

        purchase.Status = PurchaseStatus.Cancelled;
        purchase.UpdatedBy = _currentUserService.Username;
        purchase.UpdatedAtUtc = DateTime.UtcNow;

        await _purchaseRepository.UpdateAsync(purchase, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        _logger.LogInformation("Purchase order cancelled. PONumber: {PONumber}", purchase.PurchaseOrderNumber);

        return ApiResponse<bool>.SuccessResponse(true, "Purchase order cancelled successfully.");
    }

    private static PurchaseResponse MapToResponse(Purchase purchase)
    {
        return new PurchaseResponse
        {
            Id = purchase.Id,
            PurchaseOrderNumber = purchase.PurchaseOrderNumber,
            SupplierId = purchase.SupplierId,
            SupplierName = purchase.Supplier?.Name ?? string.Empty,
            SupplierCompanyName = purchase.Supplier?.CompanyName ?? string.Empty,
            SupplierCode = purchase.Supplier?.SupplierCode ?? string.Empty,
            PurchaseDateUtc = purchase.PurchaseDateUtc,
            SubTotal = purchase.SubTotal,
            TaxRate = purchase.TaxRate,
            TaxAmount = purchase.TaxAmount,
            DiscountAmount = purchase.DiscountAmount,
            TotalAmount = purchase.TotalAmount,
            PaidAmount = purchase.PaidAmount,
            DueAmount = purchase.DueAmount,
            Status = purchase.Status,
            PaymentStatus = purchase.PaymentStatus,
            Notes = purchase.Notes,
            CreatedBy = purchase.CreatedBy ?? string.Empty,
            CreatedAtUtc = purchase.CreatedAtUtc,
            Items = purchase.PurchaseItems.Select(i => new PurchaseItemResponse
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? string.Empty,
                ProductSKU = i.Product?.SKU ?? string.Empty,
                Quantity = i.Quantity,
                UnitCost = i.UnitCost,
                TotalPrice = i.TotalPrice
            }).ToList()
        };
    }
}
