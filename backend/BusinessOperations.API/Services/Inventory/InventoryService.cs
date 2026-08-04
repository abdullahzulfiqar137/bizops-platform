using BusinessOperations.API.Data;
using BusinessOperations.API.DTOs.Inventory;
using BusinessOperations.API.Entities.Inventory;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Repositories;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Services.Inventory;

public sealed class InventoryService : IInventoryService
{
    private readonly IInventoryAdjustmentRepository _adjustmentRepository;
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly IProductRepository _productRepository;
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(
        IInventoryAdjustmentRepository adjustmentRepository,
        IInventoryMovementRepository movementRepository,
        IProductRepository productRepository,
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<InventoryService> logger)
    {
        _adjustmentRepository = adjustmentRepository;
        _movementRepository = movementRepository;
        _productRepository = productRepository;
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<ApiResponse<InventoryAdjustmentResponse>> CreateAdjustmentAsync(CreateInventoryAdjustmentRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            return ApiResponse<InventoryAdjustmentResponse>.FailureResponse("Product not found.");
        }

        var previousStock = product.StockQuantity;
        decimal newStock;

        if (request.AdjustmentType == InventoryAdjustmentType.Addition)
        {
            newStock = previousStock + request.Quantity;
        }
        else
        {
            if (previousStock < request.Quantity)
            {
                return ApiResponse<InventoryAdjustmentResponse>.FailureResponse("Cannot deduct more stock than currently available.");
            }
            newStock = previousStock - request.Quantity;
        }

        var adjustmentNumber = await _adjustmentRepository.GenerateNextAdjustmentNumberAsync(cancellationToken);
        var movementNumber = await _movementRepository.GenerateNextMovementNumberAsync(cancellationToken);

        var adjustment = new InventoryAdjustment
        {
            Id = Guid.NewGuid(),
            AdjustmentNumber = adjustmentNumber,
            ProductId = request.ProductId,
            AdjustmentType = request.AdjustmentType,
            Quantity = request.Quantity,
            Reason = request.Reason,
            Notes = request.Notes,
            CreatedBy = _currentUserService.Username ?? "System",
            CreatedAtUtc = DateTime.UtcNow
        };

        var movement = new InventoryMovement
        {
            Id = Guid.NewGuid(),
            MovementNumber = movementNumber,
            ProductId = request.ProductId,
            MovementType = InventoryMovementType.Adjustment,
            Quantity = request.Quantity,
            PreviousStock = previousStock,
            NewStock = newStock,
            ReferenceType = "Adjustment",
            ReferenceId = adjustment.Id,
            Notes = $"Stock Adjustment: {request.Reason}",
            CreatedBy = _currentUserService.Username ?? "System",
            CreatedAtUtc = DateTime.UtcNow
        };

        // Update product stock
        product.StockQuantity = newStock;
        product.UpdatedBy = _currentUserService.Username;
        product.UpdatedAtUtc = DateTime.UtcNow;

        await _adjustmentRepository.AddAsync(adjustment, cancellationToken);
        await _movementRepository.AddAsync(movement, cancellationToken);
        await _productRepository.UpdateAsync(product, cancellationToken);

        _logger.LogInformation("Stock adjustment completed. AdjustmentNo: {AdjNo}, ProductId: {ProductId}, NewStock: {NewStock}",
            adjustment.AdjustmentNumber, product.Id, newStock);

        var response = new InventoryAdjustmentResponse
        {
            Id = adjustment.Id,
            AdjustmentNumber = adjustment.AdjustmentNumber,
            ProductId = product.Id,
            ProductName = product.Name,
            ProductSKU = product.SKU,
            AdjustmentType = adjustment.AdjustmentType,
            Quantity = adjustment.Quantity,
            Reason = adjustment.Reason,
            Notes = adjustment.Notes,
            CreatedBy = adjustment.CreatedBy ?? string.Empty,
            CreatedAtUtc = adjustment.CreatedAtUtc
        };

        return ApiResponse<InventoryAdjustmentResponse>.SuccessResponse(response, "Stock adjustment recorded successfully.");
    }

    public async Task<PagedResponse<InventoryAdjustmentResponse>> GetAdjustmentsAsync(
        Guid? productId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

        var (items, totalCount) = await _adjustmentRepository.GetPagedAdjustmentsAsync(
            productId, pageNumber, pageSize, cancellationToken);

        var responses = items.Select(a => new InventoryAdjustmentResponse
        {
            Id = a.Id,
            AdjustmentNumber = a.AdjustmentNumber,
            ProductId = a.ProductId,
            ProductName = a.Product?.Name ?? string.Empty,
            ProductSKU = a.Product?.SKU ?? string.Empty,
            AdjustmentType = a.AdjustmentType,
            Quantity = a.Quantity,
            Reason = a.Reason,
            Notes = a.Notes,
            CreatedBy = a.CreatedBy ?? string.Empty,
            CreatedAtUtc = a.CreatedAtUtc
        });

        return new PagedResponse<InventoryAdjustmentResponse>(responses, totalCount, pageNumber, pageSize, "Inventory adjustments retrieved successfully.");
    }

    public async Task<PagedResponse<InventoryMovementResponse>> GetMovementsAsync(
        Guid? productId,
        InventoryMovementType? movementType,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

        var (items, totalCount) = await _movementRepository.GetPagedMovementsAsync(
            productId, movementType, pageNumber, pageSize, cancellationToken);

        var responses = items.Select(m => new InventoryMovementResponse
        {
            Id = m.Id,
            MovementNumber = m.MovementNumber,
            ProductId = m.ProductId,
            ProductName = m.Product?.Name ?? string.Empty,
            ProductSKU = m.Product?.SKU ?? string.Empty,
            MovementType = m.MovementType,
            Quantity = m.Quantity,
            PreviousStock = m.PreviousStock,
            NewStock = m.NewStock,
            ReferenceType = m.ReferenceType,
            ReferenceId = m.ReferenceId,
            Notes = m.Notes,
            CreatedAtUtc = m.CreatedAtUtc
        });

        return new PagedResponse<InventoryMovementResponse>(responses, totalCount, pageNumber, pageSize, "Inventory movements retrieved successfully.");
    }

    public async Task<ApiResponse<IEnumerable<StockLevelResponse>>> GetStockLevelsAsync(Guid? categoryId, CancellationToken cancellationToken = default)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .AsNoTracking();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        var stockLevels = await query.Select(p => new StockLevelResponse
        {
            ProductId = p.Id,
            SKU = p.SKU,
            ProductName = p.Name,
            CategoryName = p.Category.Name,
            UnitAbbreviation = p.Unit.Abbreviation,
            CurrentStock = p.StockQuantity,
            MinimumStockLevel = p.MinimumStockLevel
        }).ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<StockLevelResponse>>.SuccessResponse(stockLevels, "Stock levels retrieved successfully.");
    }

    public async Task<ApiResponse<IEnumerable<StockLevelResponse>>> GetLowStockAlertsAsync(CancellationToken cancellationToken = default)
    {
        var lowStockProducts = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .AsNoTracking()
            .Where(p => p.StockQuantity <= p.MinimumStockLevel)
            .Select(p => new StockLevelResponse
            {
                ProductId = p.Id,
                SKU = p.SKU,
                ProductName = p.Name,
                CategoryName = p.Category.Name,
                UnitAbbreviation = p.Unit.Abbreviation,
                CurrentStock = p.StockQuantity,
                MinimumStockLevel = p.MinimumStockLevel
            }).ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<StockLevelResponse>>.SuccessResponse(lowStockProducts, "Low stock alerts retrieved successfully.");
    }
}
