using BusinessOperations.API.DTOs.Suppliers;
using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Suppliers;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Repositories;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Services.Suppliers;

public sealed class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SupplierService> _logger;

    public SupplierService(
        ISupplierRepository supplierRepository,
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<SupplierService> logger)
    {
        _supplierRepository = supplierRepository;
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<PagedResponse<SupplierResponse>> GetSuppliersAsync(
        string? searchTerm,
        string? city,
        bool? isActive,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

        var (items, totalCount) = await _supplierRepository.GetPagedSuppliersAsync(
            searchTerm, city, isActive, sortBy, isDescending, pageNumber, pageSize, cancellationToken);

        var responses = items.Select(MapToResponse);

        return new PagedResponse<SupplierResponse>(responses, totalCount, pageNumber, pageSize, "Suppliers retrieved successfully.");
    }

    public async Task<ApiResponse<SupplierResponse>> GetSupplierByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id, cancellationToken);
        if (supplier == null)
        {
            return ApiResponse<SupplierResponse>.FailureResponse("Supplier not found.");
        }

        return ApiResponse<SupplierResponse>.SuccessResponse(MapToResponse(supplier), "Supplier details retrieved successfully.");
    }

    public async Task<ApiResponse<SupplierResponse>> CreateSupplierAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        var code = await _supplierRepository.GenerateNextSupplierCodeAsync(cancellationToken);

        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            SupplierCode = code,
            Name = request.Name,
            CompanyName = request.CompanyName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country,
            CurrentBalance = request.OpeningBalance,
            IsActive = true,
            CreatedBy = _currentUserService.Username,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _supplierRepository.AddAsync(supplier, cancellationToken);
        _logger.LogInformation("Supplier created successfully. SupplierId: {Id}, Code: {Code}", supplier.Id, supplier.SupplierCode);

        return ApiResponse<SupplierResponse>.SuccessResponse(MapToResponse(supplier), "Supplier created successfully.");
    }

    public async Task<ApiResponse<SupplierResponse>> UpdateSupplierAsync(Guid id, UpdateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id, cancellationToken);
        if (supplier == null)
        {
            return ApiResponse<SupplierResponse>.FailureResponse("Supplier not found.");
        }

        supplier.Name = request.Name;
        supplier.CompanyName = request.CompanyName;
        supplier.Email = request.Email;
        supplier.PhoneNumber = request.PhoneNumber;
        supplier.Address = request.Address;
        supplier.City = request.City;
        supplier.State = request.State;
        supplier.PostalCode = request.PostalCode;
        supplier.Country = request.Country;
        supplier.IsActive = request.IsActive;
        supplier.UpdatedBy = _currentUserService.Username;
        supplier.UpdatedAtUtc = DateTime.UtcNow;

        await _supplierRepository.UpdateAsync(supplier, cancellationToken);
        _logger.LogInformation("Supplier updated. SupplierId: {Id}", supplier.Id);

        return ApiResponse<SupplierResponse>.SuccessResponse(MapToResponse(supplier), "Supplier updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteSupplierAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id, cancellationToken);
        if (supplier == null)
        {
            return ApiResponse<bool>.FailureResponse("Supplier not found.");
        }

        supplier.DeletedBy = _currentUserService.Username;
        await _supplierRepository.DeleteAsync(supplier, cancellationToken);
        _logger.LogInformation("Supplier soft-deleted. SupplierId: {Id}", supplier.Id);

        return ApiResponse<bool>.SuccessResponse(true, "Supplier deleted successfully.");
    }

    public async Task<ApiResponse<decimal>> GetSupplierBalanceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id, cancellationToken);
        if (supplier == null)
        {
            return ApiResponse<decimal>.FailureResponse("Supplier not found.");
        }

        return ApiResponse<decimal>.SuccessResponse(supplier.CurrentBalance, "Supplier balance retrieved successfully.");
    }

    public async Task<ApiResponse<IEnumerable<SupplierPurchaseHistoryResponse>>> GetSupplierPurchasesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id, cancellationToken);
        if (supplier == null)
        {
            return ApiResponse<IEnumerable<SupplierPurchaseHistoryResponse>>.FailureResponse("Supplier not found.");
        }

        var purchases = await _context.Purchases.AsNoTracking()
            .Where(purchase => purchase.SupplierId == id && purchase.Status != PurchaseStatus.Cancelled)
            .OrderByDescending(purchase => purchase.PurchaseDateUtc)
            .Select(purchase => new SupplierPurchaseHistoryResponse
            {
                PurchaseId = purchase.Id,
                PurchaseOrderNumber = purchase.PurchaseOrderNumber,
                TotalAmount = purchase.TotalAmount,
                PaidAmount = purchase.PaidAmount,
                RemainingBalance = purchase.DueAmount,
                PaymentStatus = purchase.PaymentStatus.ToString(),
                PurchaseDateUtc = purchase.PurchaseDateUtc
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<SupplierPurchaseHistoryResponse>>.SuccessResponse(purchases, "Supplier purchase history retrieved.");
    }

    private static SupplierResponse MapToResponse(Supplier supplier)
    {
        return new SupplierResponse
        {
            Id = supplier.Id,
            SupplierCode = supplier.SupplierCode,
            Name = supplier.Name,
            CompanyName = supplier.CompanyName,
            Email = supplier.Email,
            PhoneNumber = supplier.PhoneNumber,
            Address = supplier.Address,
            City = supplier.City,
            State = supplier.State,
            PostalCode = supplier.PostalCode,
            Country = supplier.Country,
            CurrentBalance = supplier.CurrentBalance,
            IsActive = supplier.IsActive,
            CreatedAtUtc = supplier.CreatedAtUtc
        };
    }
}
