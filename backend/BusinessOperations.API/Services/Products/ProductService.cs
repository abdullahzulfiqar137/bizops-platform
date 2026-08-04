using BusinessOperations.API.Data;
using BusinessOperations.API.DTOs.Products;
using BusinessOperations.API.Entities.Products;
using BusinessOperations.API.Interfaces.Repositories;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Services.Products;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<PagedResponse<ProductResponse>> GetProductsAsync(
        string? searchTerm,
        Guid? categoryId,
        bool? isActive,
        bool? isLowStock,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

        var (items, totalCount) = await _productRepository.GetPagedProductsAsync(
            searchTerm, categoryId, isActive, isLowStock, sortBy, isDescending, pageNumber, pageSize, cancellationToken);

        var responses = items.Select(MapToResponse);

        return new PagedResponse<ProductResponse>(responses, totalCount, pageNumber, pageSize, "Products retrieved successfully.");
    }

    public async Task<ApiResponse<ProductResponse>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetProductWithDetailsAsync(id, cancellationToken);
        if (product == null)
        {
            return ApiResponse<ProductResponse>.FailureResponse("Product not found.");
        }

        return ApiResponse<ProductResponse>.SuccessResponse(MapToResponse(product), "Product details retrieved successfully.");
    }

    public async Task<ApiResponse<ProductResponse>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        request.Barcode = NormalizeBarcode(request.Barcode);
        if (request.Barcode != null && await _context.Products.AnyAsync(x => x.Barcode == request.Barcode, cancellationToken))
            return ApiResponse<ProductResponse>.FailureResponse("This barcode is already assigned to another product.");
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
        {
            return ApiResponse<ProductResponse>.FailureResponse("Selected category does not exist.");
        }

        var unit = await _context.ProductUnits.FindAsync(new object[] { request.UnitId }, cancellationToken);
        if (unit == null)
        {
            return ApiResponse<ProductResponse>.FailureResponse("Selected product unit does not exist.");
        }

        var sku = await _productRepository.GenerateNextSKUAsync(cancellationToken);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            SKU = sku,
            Barcode = request.Barcode,
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            UnitId = request.UnitId,
            CostPrice = request.CostPrice,
            SalePrice = request.SalePrice,
            StockQuantity = request.InitialStockQuantity,
            MinimumStockLevel = request.MinimumStockLevel,
            IsActive = true,
            CreatedBy = _currentUserService.Username,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product, cancellationToken);
        _logger.LogInformation("Product created successfully. Id: {Id}, SKU: {SKU}", product.Id, product.SKU);

        var createdProduct = await _productRepository.GetProductWithDetailsAsync(product.Id, cancellationToken);
        return ApiResponse<ProductResponse>.SuccessResponse(MapToResponse(createdProduct!), "Product created successfully.");
    }

    public async Task<ApiResponse<ProductResponse>> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        request.Barcode = NormalizeBarcode(request.Barcode);
        if (request.Barcode != null && await _context.Products.AnyAsync(x => x.Id != id && x.Barcode == request.Barcode, cancellationToken))
            return ApiResponse<ProductResponse>.FailureResponse("This barcode is already assigned to another product.");
        var product = await _productRepository.GetProductWithDetailsAsync(id, cancellationToken);
        if (product == null)
        {
            return ApiResponse<ProductResponse>.FailureResponse("Product not found.");
        }

        if (product.CategoryId != request.CategoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category == null)
            {
                return ApiResponse<ProductResponse>.FailureResponse("Selected category does not exist.");
            }
            product.CategoryId = request.CategoryId;
        }

        if (product.UnitId != request.UnitId)
        {
            var unit = await _context.ProductUnits.FindAsync(new object[] { request.UnitId }, cancellationToken);
            if (unit == null)
            {
                return ApiResponse<ProductResponse>.FailureResponse("Selected product unit does not exist.");
            }
            product.UnitId = request.UnitId;
        }

        product.Name = request.Name;
        product.Barcode = request.Barcode;
        product.Description = request.Description;
        product.CostPrice = request.CostPrice;
        product.SalePrice = request.SalePrice;
        product.MinimumStockLevel = request.MinimumStockLevel;
        product.IsActive = request.IsActive;
        product.UpdatedBy = _currentUserService.Username;
        product.UpdatedAtUtc = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product, cancellationToken);
        _logger.LogInformation("Product updated. Id: {Id}", product.Id);

        var updatedProduct = await _productRepository.GetProductWithDetailsAsync(product.Id, cancellationToken);
        return ApiResponse<ProductResponse>.SuccessResponse(MapToResponse(updatedProduct!), "Product updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return ApiResponse<bool>.FailureResponse("Product not found.");
        }

        product.DeletedBy = _currentUserService.Username;
        await _productRepository.DeleteAsync(product, cancellationToken);
        _logger.LogInformation("Product soft-deleted. Id: {Id}", product.Id);

        return ApiResponse<bool>.SuccessResponse(true, "Product deleted successfully.");
    }

    public async Task<ApiResponse<IEnumerable<ProductUnitResponse>>> GetProductUnitsAsync(CancellationToken cancellationToken = default)
    {
        var units = await _context.ProductUnits
            .AsNoTracking()
            .Select(u => new ProductUnitResponse
            {
                Id = u.Id,
                Name = u.Name,
                Abbreviation = u.Abbreviation
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<ProductUnitResponse>>.SuccessResponse(units, "Product units retrieved successfully.");
    }

    private static ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            SKU = product.SKU,
            Barcode = product.Barcode,
            Name = product.Name,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            UnitId = product.UnitId,
            UnitName = product.Unit?.Name ?? string.Empty,
            UnitAbbreviation = product.Unit?.Abbreviation ?? string.Empty,
            CostPrice = product.CostPrice,
            SalePrice = product.SalePrice,
            StockQuantity = product.StockQuantity,
            MinimumStockLevel = product.MinimumStockLevel,
            IsActive = product.IsActive,
            CreatedAtUtc = product.CreatedAtUtc
        };
    }

    private static string? NormalizeBarcode(string? barcode) =>
        string.IsNullOrWhiteSpace(barcode) ? null : barcode.Trim();
}
