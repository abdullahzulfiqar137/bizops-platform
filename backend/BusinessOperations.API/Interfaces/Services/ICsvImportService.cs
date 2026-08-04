using BusinessOperations.API.DTOs.Imports;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface ICsvImportService
{
    Task<ApiResponse<CsvImportResult>> ImportCustomersAsync(IFormFile file, CancellationToken token = default);
    Task<ApiResponse<CsvImportResult>> ImportSuppliersAsync(IFormFile file, CancellationToken token = default);
    Task<ApiResponse<CsvImportResult>> ImportProductsAsync(IFormFile file, CancellationToken token = default);
}
