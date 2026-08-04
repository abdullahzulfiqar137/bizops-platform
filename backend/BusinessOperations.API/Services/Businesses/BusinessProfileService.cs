using BusinessOperations.API.Data;
using BusinessOperations.API.DTOs.Businesses;
using BusinessOperations.API.Entities.Businesses;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Services.Businesses;

public sealed class BusinessProfileService : IBusinessProfileService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<BusinessProfileService> _logger;

    public BusinessProfileService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<BusinessProfileService> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<ApiResponse<BusinessProfileResponse>> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        var profile = await _context.BusinessProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (profile == null)
        {
            profile = new BusinessProfile
            {
                Id = Guid.NewGuid(),
                CompanyName = "BizOps Enterprise",
                LegalName = "BizOps Operations Ltd.",
                CurrencyCode = "USD",
                CurrencySymbol = "$",
                TaxRate = 5.0m,
                InvoicePrefix = "INV-",
                NextInvoiceNumber = 1001,
                CreatedBy = "System",
                CreatedAtUtc = DateTime.UtcNow
            };
            _context.BusinessProfiles.Add(profile);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var response = MapToResponse(profile);
        return ApiResponse<BusinessProfileResponse>.SuccessResponse(response, "Business profile retrieved successfully.");
    }

    public async Task<ApiResponse<BusinessProfileResponse>> UpdateProfileAsync(UpdateBusinessProfileRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await _context.BusinessProfiles
            .FirstOrDefaultAsync(cancellationToken);

        if (profile == null)
        {
            profile = new BusinessProfile
            {
                Id = Guid.NewGuid(),
                CreatedBy = _currentUserService.Username
            };
            _context.BusinessProfiles.Add(profile);
        }

        profile.CompanyName = request.CompanyName;
        profile.LegalName = request.LegalName;
        profile.TaxNumber = request.TaxNumber;
        profile.Email = request.Email;
        profile.PhoneNumber = request.PhoneNumber;
        profile.Address = request.Address;
        profile.City = request.City;
        profile.State = request.State;
        profile.PostalCode = request.PostalCode;
        profile.Country = request.Country;
        profile.CurrencyCode = request.CurrencyCode;
        profile.CurrencySymbol = request.CurrencySymbol;
        profile.TaxRate = request.TaxRate;
        profile.InvoicePrefix = request.InvoicePrefix;
        profile.NextInvoiceNumber = request.NextInvoiceNumber;
        profile.LogoUrl = request.LogoUrl;
        profile.UpdatedBy = _currentUserService.Username;
        profile.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Business profile updated by user {User}. Company: {CompanyName}", _currentUserService.Username, profile.CompanyName);

        var response = MapToResponse(profile);
        return ApiResponse<BusinessProfileResponse>.SuccessResponse(response, "Business profile updated successfully.");
    }

    private static BusinessProfileResponse MapToResponse(BusinessProfile profile)
    {
        return new BusinessProfileResponse
        {
            Id = profile.Id,
            CompanyName = profile.CompanyName,
            LegalName = profile.LegalName,
            TaxNumber = profile.TaxNumber,
            Email = profile.Email,
            PhoneNumber = profile.PhoneNumber,
            Address = profile.Address,
            City = profile.City,
            State = profile.State,
            PostalCode = profile.PostalCode,
            Country = profile.Country,
            CurrencyCode = profile.CurrencyCode,
            CurrencySymbol = profile.CurrencySymbol,
            TaxRate = profile.TaxRate,
            InvoicePrefix = profile.InvoicePrefix,
            NextInvoiceNumber = profile.NextInvoiceNumber,
            LogoUrl = profile.LogoUrl,
            UpdatedAtUtc = profile.UpdatedAtUtc ?? profile.CreatedAtUtc
        };
    }
}
