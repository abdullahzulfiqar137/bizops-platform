using System.Text;
using BusinessOperations.API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/imports")]
[Authorize(Roles = "Admin,Manager")]
[RequestSizeLimit(5 * 1024 * 1024)]
public sealed class ImportsController : ControllerBase
{
    private readonly ICsvImportService _imports;
    public ImportsController(ICsvImportService imports) => _imports = imports;

    [HttpPost("customers")]
    public async Task<IActionResult> Customers(IFormFile file, CancellationToken token)
    {
        var response = await _imports.ImportCustomersAsync(file, token);
        return response.Success ? Ok(response) : BadRequest(response);
    }
    [HttpPost("suppliers")]
    public async Task<IActionResult> Suppliers(IFormFile file, CancellationToken token)
    {
        var response = await _imports.ImportSuppliersAsync(file, token);
        return response.Success ? Ok(response) : BadRequest(response);
    }
    [HttpPost("products")]
    public async Task<IActionResult> Products(IFormFile file, CancellationToken token)
    {
        var response = await _imports.ImportProductsAsync(file, token);
        return response.Success ? Ok(response) : BadRequest(response);
    }
    [HttpGet("templates/{type}")]
    public IActionResult Template(string type)
    {
        var content = type.ToLowerInvariant() switch
        {
            "customers" => "Name,Email,PhoneNumber,Address,City,State,PostalCode,Country,OpeningBalance\r\nExample Customer,customer@example.com,+920000000000,Street 1,Karachi,Sindh,75000,Pakistan,0\r\n",
            "suppliers" => "Name,CompanyName,Email,PhoneNumber,Address,City,State,PostalCode,Country,OpeningBalance\r\nContact Name,Example Supplier,supplier@example.com,+920000000000,Street 1,Lahore,Punjab,54000,Pakistan,0\r\n",
            "products" => "Name,Barcode,Description,Category,Unit,CostPrice,SalePrice,InitialStockQuantity,MinimumStockLevel\r\nExample Product,1234567890123,Example description,Electronics,pcs,100,125,10,5\r\n",
            _ => null
        };
        return content == null ? NotFound() : File(Encoding.UTF8.GetBytes(content), "text/csv", $"{type.ToLowerInvariant()}-import-template.csv");
    }
}
