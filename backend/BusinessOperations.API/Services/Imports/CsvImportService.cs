using System.Globalization;
using Microsoft.VisualBasic.FileIO;
using BusinessOperations.API.Data;
using BusinessOperations.API.DTOs.Customers;
using BusinessOperations.API.DTOs.Imports;
using BusinessOperations.API.DTOs.Products;
using BusinessOperations.API.DTOs.Suppliers;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Services.Imports;

public sealed class CsvImportService : ICsvImportService
{
    private const int MaxRows = 5000;
    private const long MaxBytes = 5 * 1024 * 1024;
    private readonly ICustomerService _customers;
    private readonly ISupplierService _suppliers;
    private readonly IProductService _products;
    private readonly ApplicationDbContext _db;
    public CsvImportService(ICustomerService customers, ISupplierService suppliers, IProductService products, ApplicationDbContext db)
    { _customers = customers; _suppliers = suppliers; _products = products; _db = db; }

    public Task<ApiResponse<CsvImportResult>> ImportCustomersAsync(IFormFile file, CancellationToken token = default) =>
        ImportAsync(file, ["Name", "Email", "PhoneNumber"], async row =>
        {
            var response = await _customers.CreateCustomerAsync(new CreateCustomerRequest
            {
                Name = row.Required("Name"), Email = row.Required("Email"), PhoneNumber = row.Required("PhoneNumber"),
                Address = row.Optional("Address"), City = row.Optional("City"), State = row.Optional("State"),
                PostalCode = row.Optional("PostalCode"), Country = row.Optional("Country"), OpeningBalance = row.Decimal("OpeningBalance")
            }, token);
            return (response.Success, response.Message);
        }, token);

    public Task<ApiResponse<CsvImportResult>> ImportSuppliersAsync(IFormFile file, CancellationToken token = default) =>
        ImportAsync(file, ["Name", "CompanyName", "Email", "PhoneNumber"], async row =>
        {
            var response = await _suppliers.CreateSupplierAsync(new CreateSupplierRequest
            {
                Name = row.Required("Name"), CompanyName = row.Required("CompanyName"), Email = row.Required("Email"), PhoneNumber = row.Required("PhoneNumber"),
                Address = row.Optional("Address"), City = row.Optional("City"), State = row.Optional("State"),
                PostalCode = row.Optional("PostalCode"), Country = row.Optional("Country"), OpeningBalance = row.Decimal("OpeningBalance")
            }, token);
            return (response.Success, response.Message);
        }, token);

    public async Task<ApiResponse<CsvImportResult>> ImportProductsAsync(IFormFile file, CancellationToken token = default)
    {
        var categories = await _db.Categories.AsNoTracking().ToDictionaryAsync(x => x.Name.ToLower(), x => x.Id, token);
        var units = await _db.ProductUnits.AsNoTracking().ToListAsync(token);
        return await ImportAsync(file, ["Name", "Category", "Unit", "CostPrice", "SalePrice"], async row =>
        {
            var categoryName = row.Required("Category").ToLower();
            if (!categories.TryGetValue(categoryName, out var categoryId)) return (false, $"Category '{row.Required("Category")}' does not exist.");
            var unitName = row.Required("Unit");
            var unit = units.FirstOrDefault(x => x.Name.Equals(unitName, StringComparison.OrdinalIgnoreCase) || x.Abbreviation.Equals(unitName, StringComparison.OrdinalIgnoreCase));
            if (unit == null) return (false, $"Unit '{unitName}' does not exist.");
            var response = await _products.CreateProductAsync(new CreateProductRequest
            {
                Name = row.Required("Name"), Barcode = row.Optional("Barcode"), Description = row.Optional("Description"),
                CategoryId = categoryId, UnitId = unit.Id, CostPrice = row.Decimal("CostPrice", true), SalePrice = row.Decimal("SalePrice", true),
                InitialStockQuantity = row.Decimal("InitialStockQuantity"), MinimumStockLevel = row.Decimal("MinimumStockLevel", defaultValue: 5)
            }, token);
            return (response.Success, response.Message);
        }, token);
    }

    private static async Task<ApiResponse<CsvImportResult>> ImportAsync(IFormFile file, string[] requiredHeaders, Func<CsvRow, Task<(bool Success, string Message)>> importer, CancellationToken token)
    {
        if (file.Length == 0 || file.Length > MaxBytes || !Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            return ApiResponse<CsvImportResult>.FailureResponse("Upload a CSV file no larger than 5 MB.");
        List<CsvRow> rows;
        try { rows = await ReadRowsAsync(file, requiredHeaders, token); }
        catch (Exception ex) when (ex is FormatException or MalformedLineException)
        { return ApiResponse<CsvImportResult>.FailureResponse(ex.Message); }
        if (rows.Count > MaxRows) return ApiResponse<CsvImportResult>.FailureResponse($"CSV files are limited to {MaxRows} data rows.");
        var result = new CsvImportResult { TotalRows = rows.Count };
        foreach (var row in rows)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                var outcome = await importer(row);
                if (outcome.Success) result.ImportedRows++;
                else result.Errors.Add(new CsvImportError { RowNumber = row.Number, Message = outcome.Message });
            }
            catch (Exception ex) when (ex is FormatException or InvalidOperationException)
            { result.Errors.Add(new CsvImportError { RowNumber = row.Number, Message = ex.Message }); }
        }
        return ApiResponse<CsvImportResult>.SuccessResponse(result, $"Imported {result.ImportedRows} of {result.TotalRows} rows.");
    }

    private static async Task<List<CsvRow>> ReadRowsAsync(IFormFile file, string[] requiredHeaders, CancellationToken token)
    {
        await using var source = file.OpenReadStream();
        using var parser = new TextFieldParser(source) { TextFieldType = FieldType.Delimited, HasFieldsEnclosedInQuotes = true, TrimWhiteSpace = true };
        parser.SetDelimiters(",");
        if (parser.EndOfData) throw new FormatException("The CSV file is empty.");
        var headers = parser.ReadFields()?.Select(x => x.Trim()).ToArray() ?? [];
        if (headers.Length == 0 || headers.Distinct(StringComparer.OrdinalIgnoreCase).Count() != headers.Length) throw new FormatException("CSV headers are empty or duplicated.");
        var missing = requiredHeaders.Where(x => !headers.Contains(x, StringComparer.OrdinalIgnoreCase)).ToList();
        if (missing.Count > 0) throw new FormatException($"Missing required CSV columns: {string.Join(", ", missing)}.");
        var rows = new List<CsvRow>(); var number = 1;
        while (!parser.EndOfData)
        {
            token.ThrowIfCancellationRequested(); number++;
            var fields = parser.ReadFields() ?? [];
            if (fields.All(string.IsNullOrWhiteSpace)) continue;
            if (fields.Length != headers.Length) throw new FormatException($"Row {number} has {fields.Length} values but {headers.Length} columns were expected.");
            rows.Add(new CsvRow(number, headers.Zip(fields).ToDictionary(x => x.First, x => x.Second, StringComparer.OrdinalIgnoreCase)));
            if (rows.Count > MaxRows) break;
        }
        await Task.CompletedTask; return rows;
    }

    private sealed record CsvRow(int Number, Dictionary<string, string> Values)
    {
        public string Required(string key) => !Values.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value) ? throw new FormatException($"{key} is required.") : value.Trim();
        public string? Optional(string key) => Values.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value) ? value.Trim() : null;
        public decimal Decimal(string key, bool required = false, decimal defaultValue = 0)
        {
            var value = Optional(key);
            if (value == null) { if (required) throw new FormatException($"{key} is required."); return defaultValue; }
            if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result) || result < 0) throw new FormatException($"{key} must be a non-negative number using '.' as the decimal separator.");
            return result;
        }
    }
}
