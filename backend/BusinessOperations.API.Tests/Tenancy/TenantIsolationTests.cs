using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Customers;
using BusinessOperations.API.Entities.Subscriptions;
using BusinessOperations.API.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BusinessOperations.API.Tests.Tenancy;

public sealed class TenantIsolationTests
{
    [Fact]
    public void TenantOwnedQuery_ContainsCurrentTenantParameter()
    {
        var tenantId = Guid.NewGuid();
        using var context = CreateContext(tenantId);

        var sql = context.Customers.ToQueryString();

        Assert.Contains(tenantId.ToString(), sql, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("CurrentTenantId", sql, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GlobalCatalogQuery_DoesNotRequireTenant()
    {
        using var context = CreateContext(null);

        var sql = context.SubscriptionPlans.ToQueryString();

        Assert.DoesNotContain("CurrentTenantId", sql, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CrossTenantWrite_IsRejectedBeforeDatabaseAccess()
    {
        var currentTenant = Guid.NewGuid();
        using var context = CreateContext(currentTenant);
        context.Customers.Add(new Customer
        {
            TenantId = Guid.NewGuid(),
            CustomerCode = "CROSS-TENANT",
            Name = "Blocked"
        });

        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            () => context.SaveChangesAsync());

        Assert.Contains("Cross-tenant", error.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static ApplicationDbContext CreateContext(Guid? tenantId)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TenantIsolationTests;Trusted_Connection=True")
            .Options;
        return new ApplicationDbContext(options, tenantContext: new StubTenantContext(tenantId));
    }

    private sealed class StubTenantContext(Guid? tenantId) : ITenantContext
    {
        public Guid? TenantId { get; private set; } = tenantId;
        public bool HasTenant => TenantId.HasValue && TenantId.Value != Guid.Empty;
        public void SetTenant(Guid value) => TenantId = value;
    }
}
