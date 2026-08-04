using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BusinessOperations.API.Interfaces.Services;

namespace BusinessOperations.API.Services.Tenancy;

public sealed class TenantContext : ITenantContext
{
    public const string TenantClaimType = "tenant_id";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Guid? _pinnedTenantId;
    private Guid? _overrideTenantId;

    public TenantContext(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        if (Guid.TryParse(configuration["Tenancy:PinnedTenantId"], out var configured))
            _pinnedTenantId = configured;
    }

    public Guid? TenantId
    {
        get
        {
            if (_overrideTenantId.HasValue) return _overrideTenantId;
            var raw = _httpContextAccessor.HttpContext?.User.FindFirstValue(TenantClaimType)
                      ?? _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Typ);
            return Guid.TryParse(raw, out var claimed) ? claimed : _pinnedTenantId;
        }
    }

    public bool HasTenant => TenantId.HasValue && TenantId.Value != Guid.Empty;

    public void SetTenant(Guid tenantId)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("Tenant ID cannot be empty.", nameof(tenantId));
        if (_overrideTenantId.HasValue && _overrideTenantId.Value != tenantId)
            throw new InvalidOperationException("The tenant context cannot be changed within a request scope.");
        _overrideTenantId = tenantId;
    }
}
