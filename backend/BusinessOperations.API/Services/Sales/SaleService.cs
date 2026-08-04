using BusinessOperations.API.Data;
using BusinessOperations.API.DTOs.Sales;
using BusinessOperations.API.Entities.Inventory;
using BusinessOperations.API.Entities.Sales;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Repositories;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Services.Sales;

public sealed class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SaleService> _logger;

    public SaleService(
        ISaleRepository saleRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IInventoryMovementRepository movementRepository,
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<SaleService> logger)
    {
        _saleRepository = saleRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _movementRepository = movementRepository;
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<ApiResponse<SaleResponse>> CreateSaleAsync(CreateSaleRequest request, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            return ApiResponse<SaleResponse>.FailureResponse("Customer not found.");
        }

        // Validate products & stock availability
        var saleItems = new List<SaleItem>();
        decimal subTotal = 0.00m;

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, cancellationToken);

        foreach (var itemReq in request.Items)
        {
            if (!products.TryGetValue(itemReq.ProductId, out var product))
            {
                return ApiResponse<SaleResponse>.FailureResponse($"Product with ID '{itemReq.ProductId}' was not found.");
            }

            if (product.StockQuantity < itemReq.Quantity)
            {
                return ApiResponse<SaleResponse>.FailureResponse($"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {itemReq.Quantity}.");
            }

            var itemTotalPrice = itemReq.Quantity * itemReq.UnitPrice;
            subTotal += itemTotalPrice;

            saleItems.Add(new SaleItem
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Quantity = itemReq.Quantity,
                UnitPrice = itemReq.UnitPrice,
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

        var invoiceNumber = await _saleRepository.GenerateNextInvoiceNumberAsync(cancellationToken);

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = invoiceNumber,
            CustomerId = customer.Id,
            SaleDateUtc = DateTime.UtcNow,
            SubTotal = subTotal,
            TaxRate = request.TaxRate,
            TaxAmount = taxAmount,
            DiscountAmount = request.DiscountAmount,
            TotalAmount = totalAmount,
            PaidAmount = request.PaidAmount,
            DueAmount = dueAmount,
            Status = SaleStatus.Completed,
            PaymentStatus = paymentStatus,
            Notes = request.Notes,
            CreatedBy = _currentUserService.Username ?? "System",
            CreatedAtUtc = DateTime.UtcNow,
            SaleItems = saleItems
        };

        // Deduct inventory & record movement audit
        foreach (var item in saleItems)
        {
            var product = products[item.ProductId];
            var previousStock = product.StockQuantity;
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
                ReferenceType = "Sale",
                ReferenceId = sale.Id,
                Notes = $"Sale Invoice: {sale.InvoiceNumber}",
                CreatedBy = _currentUserService.Username ?? "System",
                CreatedAtUtc = DateTime.UtcNow
            };

            await _movementRepository.AddAsync(movement, cancellationToken);
            await _productRepository.UpdateAsync(product, cancellationToken);
        }

        // Update customer balance if remaining due balance exists
        if (dueAmount > 0)
        {
            customer.CurrentBalance += dueAmount;
            customer.UpdatedBy = _currentUserService.Username;
            customer.UpdatedAtUtc = DateTime.UtcNow;
            await _customerRepository.UpdateAsync(customer, cancellationToken);
        }

        await _saleRepository.AddAsync(sale, cancellationToken);
        _logger.LogInformation("Sale invoice created successfully. InvoiceNo: {InvoiceNo}, Total: {TotalAmount}", sale.InvoiceNumber, sale.TotalAmount);

        var createdSale = await _saleRepository.GetSaleWithItemsAsync(sale.Id, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ApiResponse<SaleResponse>.SuccessResponse(MapToResponse(createdSale!), "Sale invoice created successfully.");
    }

    public async Task<PagedResponse<SaleResponse>> GetSalesAsync(
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
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

        var (items, totalCount) = await _saleRepository.GetPagedSalesAsync(
            searchTerm, customerId, status, paymentStatus, startDate, endDate, sortBy, isDescending, pageNumber, pageSize, cancellationToken);

        var responses = items.Select(MapToResponse);

        return new PagedResponse<SaleResponse>(responses, totalCount, pageNumber, pageSize, "Sales invoices retrieved successfully.");
    }

    public async Task<ApiResponse<SaleResponse>> GetSaleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _saleRepository.GetSaleWithItemsAsync(id, cancellationToken);
        if (sale == null)
        {
            return ApiResponse<SaleResponse>.FailureResponse("Sale invoice not found.");
        }

        return ApiResponse<SaleResponse>.SuccessResponse(MapToResponse(sale), "Sale invoice retrieved successfully.");
    }

    public async Task<Sale?> GetSaleEntityByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _saleRepository.GetSaleWithItemsAsync(id, cancellationToken);
    }

    public async Task<ApiResponse<bool>> CancelSaleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var sale = await _saleRepository.GetSaleWithItemsAsync(id, cancellationToken);
        if (sale == null)
        {
            return ApiResponse<bool>.FailureResponse("Sale invoice not found.");
        }

        if (sale.Status != SaleStatus.Completed)
        {
            return ApiResponse<bool>.FailureResponse("Only a completed invoice with no returns can be cancelled.");
        }

        // Revert stock & record movement
        foreach (var item in sale.SaleItems)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product != null)
            {
                var previousStock = product.StockQuantity;
                var newStock = previousStock + item.Quantity;

                product.StockQuantity = newStock;
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
                    ReferenceType = "SaleCancellation",
                    ReferenceId = sale.Id,
                    Notes = $"Cancelled Sale Invoice: {sale.InvoiceNumber}",
                    CreatedBy = _currentUserService.Username ?? "System",
                    CreatedAtUtc = DateTime.UtcNow
                };

                await _movementRepository.AddAsync(movement, cancellationToken);
                await _productRepository.UpdateAsync(product, cancellationToken);
            }
        }

        // Revert customer balance if due amount was added
        if (sale.DueAmount > 0)
        {
            var customer = await _customerRepository.GetByIdAsync(sale.CustomerId, cancellationToken);
            if (customer != null)
            {
                customer.CurrentBalance -= sale.DueAmount;
                customer.UpdatedBy = _currentUserService.Username;
                customer.UpdatedAtUtc = DateTime.UtcNow;
                await _customerRepository.UpdateAsync(customer, cancellationToken);
            }
        }

        sale.Status = SaleStatus.Cancelled;
        sale.UpdatedBy = _currentUserService.Username;
        sale.UpdatedAtUtc = DateTime.UtcNow;

        await _saleRepository.UpdateAsync(sale, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        _logger.LogInformation("Sale invoice cancelled. InvoiceNo: {InvoiceNo}", sale.InvoiceNumber);

        return ApiResponse<bool>.SuccessResponse(true, "Sale invoice cancelled successfully.");
    }

    private static SaleResponse MapToResponse(Sale sale)
    {
        return new SaleResponse
        {
            Id = sale.Id,
            InvoiceNumber = sale.InvoiceNumber,
            CustomerId = sale.CustomerId,
            CustomerName = sale.Customer?.Name ?? string.Empty,
            CustomerCode = sale.Customer?.CustomerCode ?? string.Empty,
            SaleDateUtc = sale.SaleDateUtc,
            SubTotal = sale.SubTotal,
            TaxRate = sale.TaxRate,
            TaxAmount = sale.TaxAmount,
            DiscountAmount = sale.DiscountAmount,
            TotalAmount = sale.TotalAmount,
            PaidAmount = sale.PaidAmount,
            DueAmount = sale.DueAmount,
            Status = sale.Status,
            PaymentStatus = sale.PaymentStatus,
            Notes = sale.Notes,
            CreatedBy = sale.CreatedBy ?? string.Empty,
            CreatedAtUtc = sale.CreatedAtUtc,
            Items = sale.SaleItems.Select(i => new SaleItemResponse
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? string.Empty,
                ProductSKU = i.Product?.SKU ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList()
        };
    }
}
