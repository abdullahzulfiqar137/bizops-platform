using BusinessOperations.API.DTOs.Products;
using BusinessOperations.API.DTOs.Reports;
using BusinessOperations.API.Interfaces.Repositories;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Services.Reports;

public sealed class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IProductRepository _productRepository;

    public ReportService(IReportRepository reportRepository, IProductRepository productRepository)
    {
        _reportRepository = reportRepository;
        _productRepository = productRepository;
    }

    public async Task<ApiResponse<DashboardSummaryResponse>> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        var summary = await _reportRepository.GetDashboardSummaryAsync(cancellationToken);
        return ApiResponse<DashboardSummaryResponse>.SuccessResponse(summary, "Dashboard summary retrieved successfully.");
    }

    public async Task<ApiResponse<SalesSummaryReportResponse>> GetSalesSummaryAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
    {
        var report = await _reportRepository.GetSalesSummaryAsync(startDate, endDate, cancellationToken);
        return ApiResponse<SalesSummaryReportResponse>.SuccessResponse(report, "Sales summary report retrieved successfully.");
    }

    public async Task<ApiResponse<PurchaseSummaryReportResponse>> GetPurchaseSummaryAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
    {
        var report = await _reportRepository.GetPurchaseSummaryAsync(startDate, endDate, cancellationToken);
        return ApiResponse<PurchaseSummaryReportResponse>.SuccessResponse(report, "Purchase summary report retrieved successfully.");
    }

    public async Task<ApiResponse<ExpenseSummaryReportResponse>> GetExpenseSummaryAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
    {
        var report = await _reportRepository.GetExpenseSummaryAsync(startDate, endDate, cancellationToken);
        return ApiResponse<ExpenseSummaryReportResponse>.SuccessResponse(report, "Expense summary report retrieved successfully.");
    }

    public async Task<ApiResponse<ProfitAndLossReportResponse>> GetProfitAndLossAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
    {
        var report = await _reportRepository.GetProfitAndLossAsync(startDate, endDate, cancellationToken);
        return ApiResponse<ProfitAndLossReportResponse>.SuccessResponse(report, "Profit and Loss statement retrieved successfully.");
    }

    public async Task<ApiResponse<IEnumerable<ProductResponse>>> GetLowStockReportAsync(CancellationToken cancellationToken = default)
    {
        var (items, _) = await _productRepository.GetPagedProductsAsync(
            searchTerm: null,
            categoryId: null,
            isActive: true,
            isLowStock: true,
            sortBy: "stockquantity",
            isDescending: false,
            pageNumber: 1,
            pageSize: 1000,
            cancellationToken: cancellationToken);

        var lowStockItems = items.Select(p => new ProductResponse
        {
            Id = p.Id,
            SKU = p.SKU,
            Name = p.Name,
            Barcode = p.Barcode,
            Description = p.Description,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? string.Empty,
            UnitId = p.UnitId,
            UnitName = p.Unit?.Name ?? string.Empty,
            UnitAbbreviation = p.Unit?.Abbreviation ?? string.Empty,
            CostPrice = p.CostPrice,
            SalePrice = p.SalePrice,
            StockQuantity = p.StockQuantity,
            MinimumStockLevel = p.MinimumStockLevel,
            IsActive = p.IsActive,
            CreatedAtUtc = p.CreatedAtUtc
        });

        return ApiResponse<IEnumerable<ProductResponse>>.SuccessResponse(lowStockItems, "Low stock products report retrieved successfully.");
    }
}
