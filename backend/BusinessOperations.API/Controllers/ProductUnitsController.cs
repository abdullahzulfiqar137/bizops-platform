using BusinessOperations.API.Data;
using BusinessOperations.API.DTOs.Products;
using BusinessOperations.API.Entities.Products;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/product-units")]
[Authorize]
public sealed class ProductUnitsController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        var units = await context.ProductUnits.AsNoTracking().OrderBy(x => x.Name)
            .Select(x => Map(x)).ToListAsync(token);
        return Ok(ApiResponse<IEnumerable<ProductUnitResponse>>.SuccessResponse(units));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken token)
    {
        var unit = await context.ProductUnits.AsNoTracking().Where(x => x.Id == id)
            .Select(x => Map(x)).SingleOrDefaultAsync(token);
        return unit == null
            ? NotFound(ApiResponse<ProductUnitResponse>.FailureResponse("Product unit not found."))
            : Ok(ApiResponse<ProductUnitResponse>.SuccessResponse(unit));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create(ProductUnitRequest request, CancellationToken token)
    {
        var name = request.Name.Trim();
        var abbreviation = request.Abbreviation.Trim();
        var error = Validate(name, abbreviation);
        if (error != null) return BadRequest(ApiResponse<ProductUnitResponse>.FailureResponse(error));
        if (await context.ProductUnits.AnyAsync(x => x.Name == name || x.Abbreviation == abbreviation, token))
            return Conflict(ApiResponse<ProductUnitResponse>.FailureResponse("A unit with that name or abbreviation already exists."));

        var unit = new ProductUnit { Name = name, Abbreviation = abbreviation };
        context.ProductUnits.Add(unit);
        await context.SaveChangesAsync(token);
        return CreatedAtAction(nameof(GetById), new { id = unit.Id }, ApiResponse<ProductUnitResponse>.SuccessResponse(Map(unit), "Product unit created."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, ProductUnitRequest request, CancellationToken token)
    {
        var unit = await context.ProductUnits.SingleOrDefaultAsync(x => x.Id == id, token);
        if (unit == null) return NotFound(ApiResponse<ProductUnitResponse>.FailureResponse("Product unit not found."));
        var name = request.Name.Trim();
        var abbreviation = request.Abbreviation.Trim();
        var error = Validate(name, abbreviation);
        if (error != null) return BadRequest(ApiResponse<ProductUnitResponse>.FailureResponse(error));
        if (await context.ProductUnits.AnyAsync(x => x.Id != id && (x.Name == name || x.Abbreviation == abbreviation), token))
            return Conflict(ApiResponse<ProductUnitResponse>.FailureResponse("A unit with that name or abbreviation already exists."));

        unit.Name = name;
        unit.Abbreviation = abbreviation;
        await context.SaveChangesAsync(token);
        return Ok(ApiResponse<ProductUnitResponse>.SuccessResponse(Map(unit), "Product unit updated."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken token)
    {
        var unit = await context.ProductUnits.SingleOrDefaultAsync(x => x.Id == id, token);
        if (unit == null) return NotFound(ApiResponse<bool>.FailureResponse("Product unit not found."));
        if (await context.Products.AnyAsync(x => x.UnitId == id, token))
            return Conflict(ApiResponse<bool>.FailureResponse("This unit is used by products and cannot be deleted."));
        context.ProductUnits.Remove(unit);
        await context.SaveChangesAsync(token);
        return Ok(ApiResponse<bool>.SuccessResponse(true, "Product unit deleted."));
    }

    private static string? Validate(string name, string abbreviation) =>
        name.Length is < 1 or > 50 ? "Name must contain between 1 and 50 characters." :
        abbreviation.Length is < 1 or > 15 ? "Abbreviation must contain between 1 and 15 characters." : null;

    private static ProductUnitResponse Map(ProductUnit unit) => new()
    {
        Id = unit.Id, Name = unit.Name, Abbreviation = unit.Abbreviation
    };
}

public sealed class ProductUnitRequest
{
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
}
