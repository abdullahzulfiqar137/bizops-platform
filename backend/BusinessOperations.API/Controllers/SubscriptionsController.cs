using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Subscriptions;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/subscriptions")]
[Authorize]
public sealed class SubscriptionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SubscriptionsController(ApplicationDbContext context) => _context = context;

    [HttpGet("plans")]
    public async Task<IActionResult> GetPlans(CancellationToken cancellationToken)
    {
        var plans = await _context.SubscriptionPlans.AsNoTracking()
            .Where(plan => plan.IsActive)
            .OrderBy(plan => plan.MonthlyPrice)
            .ToListAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<SubscriptionPlan>>.SuccessResponse(plans));
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentSubscription(CancellationToken cancellationToken)
    {
        var subscription = await _context.TenantSubscriptions.AsNoTracking()
            .Include(item => item.Plan)
            .OrderByDescending(item => item.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (subscription == null)
            return Ok(ApiResponse<object?>.SuccessResponse(null, "No subscription exists for this tenant."));

        var activeUsers = await _context.Users.CountAsync(user => user.IsActive, cancellationToken);
        var periodStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthlySales = await _context.Sales.CountAsync(sale => sale.SaleDateUtc >= periodStart, cancellationToken);

        return Ok(ApiResponse<object>.SuccessResponse(new
        {
            subscription,
            usage = new
            {
                activeUsers,
                maxUsers = subscription.Plan?.MaxUsers ?? 0,
                monthlySales,
                maxMonthlySales = subscription.Plan?.MaxSalesPerMonth ?? 0
            }
        }));
    }

    [HttpPost("subscribe")]
    [Authorize(Roles = "Admin")]
    public IActionResult Subscribe([FromBody] SubscribeRequest request)
    {
        // Never grant a paid plan without a verified payment-provider webhook.
        return StatusCode(StatusCodes.Status501NotImplemented,
            ApiResponse<object>.FailureResponse(
                "Billing provider integration is not configured. No subscription was changed and no payment was recorded."));
    }

    [HttpGet("invoices")]
    public async Task<IActionResult> GetInvoices(CancellationToken cancellationToken)
    {
        var invoices = await _context.SubscriptionInvoices.AsNoTracking()
            .Include(invoice => invoice.Subscription)
            .ThenInclude(subscription => subscription!.Plan)
            .OrderByDescending(invoice => invoice.BillingDateUtc)
            .ToListAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<SubscriptionInvoice>>.SuccessResponse(invoices));
    }
}

public sealed class SubscribeRequest
{
    public Guid PlanId { get; set; }
    public bool IsAnnual { get; set; } = true;
}
