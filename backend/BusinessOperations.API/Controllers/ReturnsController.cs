using BusinessOperations.API.DTOs.Purchases;
using BusinessOperations.API.DTOs.Sales;
using BusinessOperations.API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public sealed class ReturnsController : ControllerBase
{
    private readonly IReturnService _returns;
    public ReturnsController(IReturnService returns) => _returns = returns;

    [HttpPost("sales/{saleId:guid}/returns")]
    public async Task<IActionResult> CreateSaleReturn(Guid saleId, CreateSaleReturnRequest request, CancellationToken token)
    {
        var response = await _returns.CreateSaleReturnAsync(saleId, request, token);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("sales/{saleId:guid}/returns")]
    public async Task<IActionResult> GetSaleReturns(Guid saleId, CancellationToken token) =>
        Ok(await _returns.GetSaleReturnsAsync(saleId, token));

    [HttpPost("purchases/{purchaseId:guid}/returns")]
    public async Task<IActionResult> CreatePurchaseReturn(Guid purchaseId, CreatePurchaseReturnRequest request, CancellationToken token)
    {
        var response = await _returns.CreatePurchaseReturnAsync(purchaseId, request, token);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("purchases/{purchaseId:guid}/returns")]
    public async Task<IActionResult> GetPurchaseReturns(Guid purchaseId, CancellationToken token) =>
        Ok(await _returns.GetPurchaseReturnsAsync(purchaseId, token));
}
