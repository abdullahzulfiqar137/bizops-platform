using BusinessOperations.API.Data;
using BusinessOperations.API.DTOs.Purchases;
using BusinessOperations.API.DTOs.Sales;
using BusinessOperations.API.Entities.Inventory;
using BusinessOperations.API.Entities.Purchases;
using BusinessOperations.API.Entities.Sales;
using BusinessOperations.API.Entities.Products;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Repositories;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Services.Returns;

public sealed class ReturnService : IReturnService
{
    private readonly ApplicationDbContext _context;
    private readonly IInventoryMovementRepository _movements;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<ReturnService> _logger;

    public ReturnService(ApplicationDbContext context, IInventoryMovementRepository movements, ICurrentUserService currentUser, ILogger<ReturnService> logger)
    {
        _context = context;
        _movements = movements;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ApiResponse<SaleReturnResponse>> CreateSaleReturnAsync(Guid saleId, CreateSaleReturnRequest request, CancellationToken cancellationToken = default)
    {
        var validation = ValidateRequest(request.Reason, request.Items.Select(x => (x.SaleItemId, x.Quantity)));
        if (validation != null) return ApiResponse<SaleReturnResponse>.FailureResponse(validation);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var sale = await _context.Sales
            .Include(x => x.Customer).Include(x => x.SaleItems).ThenInclude(x => x.Product)
            .SingleOrDefaultAsync(x => x.Id == saleId, cancellationToken);
        if (sale == null) return ApiResponse<SaleReturnResponse>.FailureResponse("Sale invoice not found.");
        if (sale.Status is SaleStatus.Cancelled or SaleStatus.Returned)
            return ApiResponse<SaleReturnResponse>.FailureResponse("This invoice cannot accept another return.");

        var prior = await _context.SaleReturnItems
            .Where(x => x.SaleReturn.SaleId == saleId)
            .GroupBy(x => x.SaleItemId)
            .Select(x => new { Id = x.Key, Quantity = x.Sum(y => y.Quantity) })
            .ToDictionaryAsync(x => x.Id, x => x.Quantity, cancellationToken);

        var result = new SaleReturn
        {
            Id = Guid.NewGuid(), SaleId = sale.Id, ReturnNumber = NextNumber("SR"),
            Reason = request.Reason.Trim(), Notes = request.Notes?.Trim(), CreatedBy = _currentUser.Username ?? "System"
        };
        foreach (var requested in request.Items)
        {
            var item = sale.SaleItems.SingleOrDefault(x => x.Id == requested.SaleItemId);
            if (item == null) return ApiResponse<SaleReturnResponse>.FailureResponse("A selected item does not belong to this invoice.");
            var available = item.Quantity - prior.GetValueOrDefault(item.Id);
            if (requested.Quantity > available)
                return ApiResponse<SaleReturnResponse>.FailureResponse($"Return quantity for '{item.Product.Name}' exceeds the remaining returnable quantity ({available}).");

            var lineTotal = Math.Round(requested.Quantity * item.UnitPrice, 2);
            result.Items.Add(new SaleReturnItem
            {
                Id = Guid.NewGuid(), SaleItemId = item.Id, ProductId = item.ProductId,
                Quantity = requested.Quantity, UnitPrice = item.UnitPrice, TotalPrice = lineTotal, Restock = requested.Restock
            });
            result.SubTotal += lineTotal;
            if (requested.Restock)
                await AddMovementAsync(item.Product, requested.Quantity, InventoryMovementType.In, "SaleReturn", result.Id, result.ReturnNumber, cancellationToken);
        }

        ApplyAmounts(result, sale.SubTotal, sale.TaxAmount, sale.DiscountAmount);
        var previouslyCredited = await _context.SaleReturns.Where(x => x.SaleId == saleId).SumAsync(x => x.CustomerCreditApplied, cancellationToken);
        var creditableDue = Math.Max(0, sale.DueAmount - previouslyCredited);
        result.CustomerCreditApplied = Math.Min(result.TotalAmount, creditableDue);
        result.RefundAmount = result.TotalAmount - result.CustomerCreditApplied;
        sale.Customer.CurrentBalance = Math.Max(0, sale.Customer.CurrentBalance - result.CustomerCreditApplied);

        _context.SaleReturns.Add(result);
        sale.Status = await IsFullyReturnedSaleAsync(sale, request.Items, prior) ? SaleStatus.Returned : SaleStatus.PartiallyReturned;
        sale.UpdatedBy = _currentUser.Username;
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        _logger.LogInformation("Sale return {ReturnNumber} created for {InvoiceNumber}", result.ReturnNumber, sale.InvoiceNumber);
        return ApiResponse<SaleReturnResponse>.SuccessResponse(Map(result, sale.InvoiceNumber), "Sales return created successfully.");
    }

    public async Task<ApiResponse<IReadOnlyList<SaleReturnResponse>>> GetSaleReturnsAsync(Guid saleId, CancellationToken cancellationToken = default)
    {
        var records = await _context.SaleReturns.AsNoTracking().Include(x => x.Sale).Include(x => x.Items).ThenInclude(x => x.Product)
            .Where(x => x.SaleId == saleId).OrderByDescending(x => x.ReturnDateUtc).ToListAsync(cancellationToken);
        return ApiResponse<IReadOnlyList<SaleReturnResponse>>.SuccessResponse(records.Select(x => Map(x, x.Sale.InvoiceNumber)).ToList(), "Sales returns retrieved successfully.");
    }

    public async Task<ApiResponse<PurchaseReturnResponse>> CreatePurchaseReturnAsync(Guid purchaseId, CreatePurchaseReturnRequest request, CancellationToken cancellationToken = default)
    {
        var validation = ValidateRequest(request.Reason, request.Items.Select(x => (x.PurchaseItemId, x.Quantity)));
        if (validation != null) return ApiResponse<PurchaseReturnResponse>.FailureResponse(validation);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var purchase = await _context.Purchases
            .Include(x => x.Supplier).Include(x => x.PurchaseItems).ThenInclude(x => x.Product)
            .SingleOrDefaultAsync(x => x.Id == purchaseId, cancellationToken);
        if (purchase == null) return ApiResponse<PurchaseReturnResponse>.FailureResponse("Purchase order not found.");
        if (purchase.Status is PurchaseStatus.Cancelled or PurchaseStatus.Returned)
            return ApiResponse<PurchaseReturnResponse>.FailureResponse("This purchase cannot accept another return.");

        var prior = await _context.PurchaseReturnItems.Where(x => x.PurchaseReturn.PurchaseId == purchaseId)
            .GroupBy(x => x.PurchaseItemId).Select(x => new { Id = x.Key, Quantity = x.Sum(y => y.Quantity) })
            .ToDictionaryAsync(x => x.Id, x => x.Quantity, cancellationToken);
        var result = new PurchaseReturn
        {
            Id = Guid.NewGuid(), PurchaseId = purchase.Id, ReturnNumber = NextNumber("PR"),
            Reason = request.Reason.Trim(), Notes = request.Notes?.Trim(), CreatedBy = _currentUser.Username ?? "System"
        };
        foreach (var requested in request.Items)
        {
            var item = purchase.PurchaseItems.SingleOrDefault(x => x.Id == requested.PurchaseItemId);
            if (item == null) return ApiResponse<PurchaseReturnResponse>.FailureResponse("A selected item does not belong to this purchase.");
            var available = item.Quantity - prior.GetValueOrDefault(item.Id);
            if (requested.Quantity > available)
                return ApiResponse<PurchaseReturnResponse>.FailureResponse($"Return quantity for '{item.Product.Name}' exceeds the remaining returnable quantity ({available}).");
            if (item.Product.StockQuantity < requested.Quantity)
                return ApiResponse<PurchaseReturnResponse>.FailureResponse($"Insufficient stock to return '{item.Product.Name}'. Available: {item.Product.StockQuantity}.");

            var lineTotal = Math.Round(requested.Quantity * item.UnitCost, 2);
            result.Items.Add(new PurchaseReturnItem { Id = Guid.NewGuid(), PurchaseItemId = item.Id, ProductId = item.ProductId, Quantity = requested.Quantity, UnitCost = item.UnitCost, TotalPrice = lineTotal });
            result.SubTotal += lineTotal;
            await AddMovementAsync(item.Product, requested.Quantity, InventoryMovementType.Out, "PurchaseReturn", result.Id, result.ReturnNumber, cancellationToken);
        }
        ApplyAmounts(result, purchase.SubTotal, purchase.TaxAmount, purchase.DiscountAmount);
        var previouslyCredited = await _context.PurchaseReturns.Where(x => x.PurchaseId == purchaseId).SumAsync(x => x.SupplierCreditApplied, cancellationToken);
        var creditableDue = Math.Max(0, purchase.DueAmount - previouslyCredited);
        result.SupplierCreditApplied = Math.Min(result.TotalAmount, creditableDue);
        result.RefundAmount = result.TotalAmount - result.SupplierCreditApplied;
        purchase.Supplier.CurrentBalance = Math.Max(0, purchase.Supplier.CurrentBalance - result.SupplierCreditApplied);
        _context.PurchaseReturns.Add(result);
        purchase.Status = await IsFullyReturnedPurchaseAsync(purchase, request.Items, prior) ? PurchaseStatus.Returned : PurchaseStatus.PartiallyReturned;
        purchase.UpdatedBy = _currentUser.Username;
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        _logger.LogInformation("Purchase return {ReturnNumber} created for {PurchaseNumber}", result.ReturnNumber, purchase.PurchaseOrderNumber);
        return ApiResponse<PurchaseReturnResponse>.SuccessResponse(Map(result, purchase.PurchaseOrderNumber), "Purchase return created successfully.");
    }

    public async Task<ApiResponse<IReadOnlyList<PurchaseReturnResponse>>> GetPurchaseReturnsAsync(Guid purchaseId, CancellationToken cancellationToken = default)
    {
        var records = await _context.PurchaseReturns.AsNoTracking().Include(x => x.Purchase).Include(x => x.Items).ThenInclude(x => x.Product)
            .Where(x => x.PurchaseId == purchaseId).OrderByDescending(x => x.ReturnDateUtc).ToListAsync(cancellationToken);
        return ApiResponse<IReadOnlyList<PurchaseReturnResponse>>.SuccessResponse(records.Select(x => Map(x, x.Purchase.PurchaseOrderNumber)).ToList(), "Purchase returns retrieved successfully.");
    }

    private async Task AddMovementAsync(Product product, decimal quantity, InventoryMovementType type, string referenceType, Guid referenceId, string number, CancellationToken token)
    {
        var previous = product.StockQuantity;
        product.StockQuantity += type == InventoryMovementType.In ? quantity : -quantity;
        product.UpdatedBy = _currentUser.Username;
        _context.InventoryMovements.Add(new InventoryMovement
        {
            Id = Guid.NewGuid(), MovementNumber = await _movements.GenerateNextMovementNumberAsync(token), ProductId = product.Id,
            MovementType = type, Quantity = quantity, PreviousStock = previous, NewStock = product.StockQuantity,
            ReferenceType = referenceType, ReferenceId = referenceId, Notes = $"{referenceType}: {number}", CreatedBy = _currentUser.Username ?? "System"
        });
    }

    private static string NextNumber(string prefix) =>
        $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}"[..30].ToUpperInvariant();

    private static string? ValidateRequest(string reason, IEnumerable<(Guid Id, decimal Quantity)> items)
    {
        var list = items.ToList();
        if (string.IsNullOrWhiteSpace(reason)) return "A return reason is required.";
        if (reason.Length > 250) return "The return reason must not exceed 250 characters.";
        if (list.Count == 0) return "Select at least one item to return.";
        if (list.Any(x => x.Id == Guid.Empty || x.Quantity <= 0)) return "Every returned item requires a valid positive quantity.";
        if (list.GroupBy(x => x.Id).Any(x => x.Count() > 1)) return "The same item cannot appear more than once.";
        return null;
    }

    private static void ApplyAmounts(SaleReturn value, decimal originalSubTotal, decimal tax, decimal discount)
    {
        var ratio = originalSubTotal <= 0 ? 0 : value.SubTotal / originalSubTotal;
        value.TaxAmount = Math.Round(tax * ratio, 2);
        value.DiscountAmount = Math.Round(discount * ratio, 2);
        value.TotalAmount = Math.Max(0, value.SubTotal + value.TaxAmount - value.DiscountAmount);
    }
    private static void ApplyAmounts(PurchaseReturn value, decimal originalSubTotal, decimal tax, decimal discount)
    {
        var ratio = originalSubTotal <= 0 ? 0 : value.SubTotal / originalSubTotal;
        value.TaxAmount = Math.Round(tax * ratio, 2);
        value.DiscountAmount = Math.Round(discount * ratio, 2);
        value.TotalAmount = Math.Max(0, value.SubTotal + value.TaxAmount - value.DiscountAmount);
    }
    private static Task<bool> IsFullyReturnedSaleAsync(Sale sale, IEnumerable<CreateSaleReturnItemRequest> current, IReadOnlyDictionary<Guid, decimal> prior)
    {
        var now = current.ToDictionary(x => x.SaleItemId, x => x.Quantity);
        return Task.FromResult(sale.SaleItems.All(x => prior.GetValueOrDefault(x.Id) + now.GetValueOrDefault(x.Id) >= x.Quantity));
    }
    private static Task<bool> IsFullyReturnedPurchaseAsync(Purchase purchase, IEnumerable<CreatePurchaseReturnItemRequest> current, IReadOnlyDictionary<Guid, decimal> prior)
    {
        var now = current.ToDictionary(x => x.PurchaseItemId, x => x.Quantity);
        return Task.FromResult(purchase.PurchaseItems.All(x => prior.GetValueOrDefault(x.Id) + now.GetValueOrDefault(x.Id) >= x.Quantity));
    }
    private static SaleReturnResponse Map(SaleReturn x, string invoice) => new()
    {
        Id = x.Id, ReturnNumber = x.ReturnNumber, SaleId = x.SaleId, InvoiceNumber = invoice, ReturnDateUtc = x.ReturnDateUtc,
        SubTotal = x.SubTotal, TaxAmount = x.TaxAmount, DiscountAmount = x.DiscountAmount, TotalAmount = x.TotalAmount,
        CustomerCreditApplied = x.CustomerCreditApplied, RefundAmount = x.RefundAmount, Reason = x.Reason, Notes = x.Notes,
        Items = x.Items.Select(i => new SaleReturnItemResponse { SaleItemId = i.SaleItemId, ProductId = i.ProductId, ProductName = i.Product?.Name ?? "", Quantity = i.Quantity, UnitPrice = i.UnitPrice, TotalPrice = i.TotalPrice, Restocked = i.Restock }).ToList()
    };
    private static PurchaseReturnResponse Map(PurchaseReturn x, string number) => new()
    {
        Id = x.Id, ReturnNumber = x.ReturnNumber, PurchaseId = x.PurchaseId, PurchaseOrderNumber = number, ReturnDateUtc = x.ReturnDateUtc,
        SubTotal = x.SubTotal, TaxAmount = x.TaxAmount, DiscountAmount = x.DiscountAmount, TotalAmount = x.TotalAmount,
        SupplierCreditApplied = x.SupplierCreditApplied, RefundAmount = x.RefundAmount, Reason = x.Reason, Notes = x.Notes,
        Items = x.Items.Select(i => new PurchaseReturnItemResponse { PurchaseItemId = i.PurchaseItemId, ProductId = i.ProductId, ProductName = i.Product?.Name ?? "", Quantity = i.Quantity, UnitCost = i.UnitCost, TotalPrice = i.TotalPrice }).ToList()
    };
}
