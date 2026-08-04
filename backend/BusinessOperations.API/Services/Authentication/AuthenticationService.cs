using System.Security.Claims;
using BusinessOperations.API.Data;
using BusinessOperations.API.DTOs.Authentication;
using BusinessOperations.API.DTOs.Users;
using BusinessOperations.API.Entities.Identity;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text;
using BusinessOperations.API.Configurations;
using BusinessOperations.API.Entities.Tenancy;
using System.Text.RegularExpressions;

namespace BusinessOperations.API.Services.Authentication;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ITokenGeneratorService _tokenGeneratorService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;
    private readonly ITenantContext _tenantContext;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ITokenGeneratorService tokenGeneratorService,
        ICurrentUserService currentUserService,
        ApplicationDbContext context,
        ILogger<AuthenticationService> logger,
        IEmailService emailService,
        IOptions<EmailSettings> emailSettings,
        ITenantContext tenantContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenGeneratorService = tokenGeneratorService;
        _currentUserService = currentUserService;
        _context = context;
        _logger = logger;
        _emailService = emailService;
        _emailSettings = emailSettings.Value;
        _tenantContext = tenantContext;
    }

    public async Task<ApiResponse<UserResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingEmail != null)
        {
            return ApiResponse<UserResponse>.FailureResponse("Email address is already registered.");
        }

        var existingUsername = await _userManager.FindByNameAsync(request.Username);
        if (existingUsername != null)
        {
            return ApiResponse<UserResponse>.FailureResponse("Username is already taken.");
        }

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.CompanyName.Trim(),
            Slug = await CreateUniqueTenantSlugAsync(request.CompanyName, cancellationToken)
        };
        _tenantContext.SetTenant(tenant.Id);

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Username,
            PhoneNumber = request.PhoneNumber,
            TenantId = tenant.Id,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            await transaction.RollbackAsync(cancellationToken);
            var errors = result.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

            return ApiResponse<UserResponse>.FailureResponse("User registration failed.", errors);
        }

        // The creator is the tenant administrator; additional users are invited by that admin.
        const string defaultRole = "Admin";
        if (await _roleManager.RoleExistsAsync(defaultRole))
        {
            await _userManager.AddToRoleAsync(user, defaultRole);
        }

        var starterPlan = await _context.SubscriptionPlans
            .IgnoreQueryFilters()
            .OrderBy(x => x.MonthlyPrice)
            .FirstOrDefaultAsync(cancellationToken);
        if (starterPlan != null)
        {
            _context.TenantSubscriptions.Add(new BusinessOperations.API.Entities.Subscriptions.TenantSubscription
            {
                PlanId = starterPlan.Id,
                Status = BusinessOperations.API.Entities.Subscriptions.SubscriptionStatus.Trialing,
                StartDateUtc = DateTime.UtcNow,
                TrialEndsAtUtc = DateTime.UtcNow.AddDays(14),
                CreatedBy = user.UserName
            });
            await _context.SaveChangesAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);

        var roles = await _userManager.GetRolesAsync(user);
        var response = MapToUserResponse(user, roles);

        _logger.LogInformation("User registered successfully. UserId: {UserId}, Email: {Email}", user.Id, user.Email);

        return ApiResponse<UserResponse>.SuccessResponse(response, "User registered successfully.");
    }

    private async Task<string> CreateUniqueTenantSlugAsync(string companyName, CancellationToken cancellationToken)
    {
        var baseSlug = Regex.Replace(companyName.Trim().ToLowerInvariant(), "[^a-z0-9]+", "-").Trim('-');
        if (string.IsNullOrWhiteSpace(baseSlug)) baseSlug = "tenant";
        if (baseSlug.Length > 80) baseSlug = baseSlug[..80].TrimEnd('-');

        var candidate = baseSlug;
        var suffix = 1;
        while (await _context.Tenants.AnyAsync(x => x.Slug == candidate, cancellationToken))
            candidate = $"{baseSlug}-{++suffix}";
        return candidate;
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, string ipAddress, string? userAgent, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.UsernameOrEmail)
            ?? await _userManager.FindByNameAsync(request.UsernameOrEmail);

        if (user == null)
        {
            return ApiResponse<AuthResponse>.FailureResponse("Invalid credentials.");
        }

        _tenantContext.SetTenant(user.TenantId);

        if (!user.IsActive)
        {
            return ApiResponse<AuthResponse>.FailureResponse("Account is deactivated. Please contact an administrator.");
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            _logger.LogWarning(
                "Login blocked for locked account. UserId: {UserId}",
                user.Id);
            return ApiResponse<AuthResponse>.FailureResponse(
                "Invalid credentials or the account is temporarily locked.");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            await _userManager.AccessFailedAsync(user);
            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning(
                    "Account locked after repeated failed sign-in attempts. UserId: {UserId}",
                    user.Id);
            }
            return ApiResponse<AuthResponse>.FailureResponse("Invalid credentials.");
        }

        if (await _userManager.GetTwoFactorEnabledAsync(user))
        {
            var code = request.TwoFactorCode?.Replace(" ", string.Empty).Replace("-", string.Empty);
            if (string.IsNullOrWhiteSpace(code) ||
                !await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, code))
            {
                return ApiResponse<AuthResponse>.FailureResponse(
                    string.IsNullOrWhiteSpace(code) ? "Two-factor authentication code is required." : "Invalid two-factor authentication code.");
            }
        }

        await _userManager.ResetAccessFailedCountAsync(user);
        var (accessToken, accessExpires) = await _tokenGeneratorService.GenerateAccessTokenAsync(user);
        var refreshToken = _tokenGeneratorService.GenerateRefreshToken(user, ipAddress);
        refreshToken.UserAgent = userAgent;

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        var roles = await _userManager.GetRolesAsync(user);
        var userResponse = MapToUserResponse(user, roles);

        var authResponse = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshTokenExpiresAtUtc = refreshToken.ExpiresAtUtc,
            User = userResponse
        };

        _logger.LogInformation("User logged in successfully. UserId: {UserId}", user.Id);

        return ApiResponse<AuthResponse>.SuccessResponse(authResponse, "Login successful.");
    }

    public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, string? userAgent, CancellationToken cancellationToken = default)
    {
        ClaimsPrincipal principal;
        try
        {
            principal = _tokenGeneratorService.GetPrincipalFromExpiredToken(request.AccessToken)!;
        }
        catch
        {
            return ApiResponse<AuthResponse>.FailureResponse("Invalid access token.");
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return ApiResponse<AuthResponse>.FailureResponse("Invalid token claims.");
        }

        var existingRefreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && rt.UserId == userId, cancellationToken);

        if (existingRefreshToken == null || !existingRefreshToken.IsActive)
        {
            return ApiResponse<AuthResponse>.FailureResponse("Invalid or expired refresh token.");
        }

        // Revoke old token and rotate
        existingRefreshToken.RevokedAtUtc = DateTime.UtcNow;
        existingRefreshToken.RevokedByIp = ipAddress;

        var user = existingRefreshToken.User;
        if (!user.IsActive)
        {
            return ApiResponse<AuthResponse>.FailureResponse("Account is deactivated.");
        }

        var (newAccessToken, accessExpires) = await _tokenGeneratorService.GenerateAccessTokenAsync(user);
        var newRefreshToken = _tokenGeneratorService.GenerateRefreshToken(user, ipAddress);
        newRefreshToken.UserAgent = userAgent;

        existingRefreshToken.ReplacedByToken = newRefreshToken.Token;
        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        var roles = await _userManager.GetRolesAsync(user);
        var userResponse = MapToUserResponse(user, roles);

        var authResponse = new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshTokenExpiresAtUtc = newRefreshToken.ExpiresAtUtc,
            User = userResponse
        };

        return ApiResponse<AuthResponse>.SuccessResponse(authResponse, "Token refreshed successfully.");
    }

    public async Task<ApiResponse<bool>> RevokeTokenAsync(RevokeTokenRequest request, string ipAddress, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshToken == null || !refreshToken.IsActive)
        {
            return ApiResponse<bool>.FailureResponse("Invalid or inactive refresh token.");
        }

        refreshToken.RevokedAtUtc = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true, "Token revoked successfully.");
    }

    public async Task<ApiResponse<bool>> ForgotPasswordAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var inputEmail = request.Email.Trim();
        var user = await _userManager.FindByEmailAsync(inputEmail);

        if (user == null || string.IsNullOrWhiteSpace(user.Email))
        {
            _logger.LogInformation("Password recovery requested for an unknown email address.");
            return ApiResponse<bool>.SuccessResponse(true, "If the account exists, password reset instructions will be sent.");
        }

        if (!user.IsActive)
        {
            return ApiResponse<bool>.SuccessResponse(true, "If the account exists, password reset instructions will be sent.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var resetUrl =
            $"{_emailSettings.FrontendBaseUrl.TrimEnd('/')}/auth/reset-password" +
            $"?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(encodedToken)}";

        try
        {
            await _emailService.SendPasswordResetAsync(
                user.Email,
                $"{user.FirstName} {user.LastName}".Trim(),
                resetUrl,
                cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogError(exception, "Password reset email delivery failed for UserId: {UserId}", user.Id);
            return ApiResponse<bool>.SuccessResponse(true, "If the account exists, password reset instructions will be sent.");
        }

        _logger.LogInformation("Password reset token generated and email sent to {Email} for UserId: {UserId}", user.Email, user.Id);
        return ApiResponse<bool>.SuccessResponse(true, "If the account exists, password reset instructions will be sent.");
    }

    public async Task<ApiResponse<bool>> ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (user == null || !user.IsActive)
        {
            return ApiResponse<bool>.FailureResponse("The reset link is invalid or expired.");
        }

        string token;
        try
        {
            token = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(request.Token));
        }
        catch (FormatException)
        {
            return ApiResponse<bool>.FailureResponse("The reset link is invalid or expired.");
        }

        var result = await _userManager.ResetPasswordAsync(
            user,
            token,
            request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .GroupBy(error => error.Code)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.Description).ToArray());
            return ApiResponse<bool>.FailureResponse(
                "Password reset failed. The link may be expired or the password does not meet security requirements.",
                errors);
        }

        var activeRefreshTokens = await _context.RefreshTokens
            .Where(refreshToken =>
                refreshToken.UserId == user.Id &&
                refreshToken.RevokedAtUtc == null &&
                refreshToken.ExpiresAtUtc > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var refreshToken in activeRefreshTokens)
        {
            refreshToken.RevokedAtUtc = DateTime.UtcNow;
            refreshToken.RevokedByIp = "password-reset";
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _userManager.UpdateSecurityStampAsync(user);
        _logger.LogInformation("Password reset completed for UserId: {UserId}", user.Id);

        return ApiResponse<bool>.SuccessResponse(
            true,
            "Password updated successfully. Sign in with your new password.");
    }

    public async Task<ApiResponse<UserResponse>> GetCurrentUserProfileAsync(CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == null)
        {
            return ApiResponse<UserResponse>.FailureResponse("User is not authenticated.");
        }

        var user = await _userManager.FindByIdAsync(_currentUserService.UserId.Value.ToString());
        if (user == null)
        {
            return ApiResponse<UserResponse>.FailureResponse("User profile not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var response = MapToUserResponse(user, roles);

        return ApiResponse<UserResponse>.SuccessResponse(response, "User profile retrieved successfully.");
    }

    public async Task<ApiResponse<UserResponse>> UpdateCurrentUserProfileAsync(
        UpdateMyProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await GetAuthenticatedUserAsync();
        if (string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName) ||
            string.IsNullOrWhiteSpace(request.Email))
            return ApiResponse<UserResponse>.FailureResponse("First name, last name, and email are required.");

        var emailOwner = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (emailOwner != null && emailOwner.Id != user.Id)
            return ApiResponse<UserResponse>.FailureResponse("Email address is already used by another account.");

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Email = request.Email.Trim();
        user.NormalizedEmail = _userManager.NormalizeEmail(user.Email);
        user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
        user.UpdatedAtUtc = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return ApiResponse<UserResponse>.FailureResponse(
                "Profile could not be updated.",
                result.Errors.GroupBy(x => x.Code).ToDictionary(x => x.Key, x => x.Select(e => e.Description).ToArray()));

        var roles = await _userManager.GetRolesAsync(user);
        return ApiResponse<UserResponse>.SuccessResponse(MapToUserResponse(user, roles), "Profile updated successfully.");
    }

    public async Task<ApiResponse<TwoFactorSetupResponse>> GetTwoFactorSetupAsync(CancellationToken cancellationToken = default)
    {
        var user = await GetAuthenticatedUserAsync();
        var key = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrWhiteSpace(key))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            key = await _userManager.GetAuthenticatorKeyAsync(user) ?? string.Empty;
        }
        var issuer = Uri.EscapeDataString("AI BizOps");
        var account = Uri.EscapeDataString(user.Email ?? user.UserName ?? user.Id.ToString());
        return ApiResponse<TwoFactorSetupResponse>.SuccessResponse(new TwoFactorSetupResponse
        {
            IsEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
            SharedKey = key,
            AuthenticatorUri = $"otpauth://totp/{issuer}:{account}?secret={key}&issuer={issuer}&digits=6"
        });
    }

    public async Task<ApiResponse<bool>> EnableTwoFactorAsync(VerifyTwoFactorRequest request, CancellationToken cancellationToken = default)
    {
        var user = await GetAuthenticatedUserAsync();
        var valid = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, NormalizeCode(request.Code));
        if (!valid) return ApiResponse<bool>.FailureResponse("Invalid authenticator code.");
        await _userManager.SetTwoFactorEnabledAsync(user, true);
        return ApiResponse<bool>.SuccessResponse(true, "Two-factor authentication enabled.");
    }

    public async Task<ApiResponse<bool>> DisableTwoFactorAsync(VerifyTwoFactorRequest request, CancellationToken cancellationToken = default)
    {
        var user = await GetAuthenticatedUserAsync();
        var valid = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, NormalizeCode(request.Code));
        if (!valid) return ApiResponse<bool>.FailureResponse("Invalid authenticator code.");
        await _userManager.SetTwoFactorEnabledAsync(user, false);
        await _userManager.ResetAuthenticatorKeyAsync(user);
        return ApiResponse<bool>.SuccessResponse(true, "Two-factor authentication disabled.");
    }

    public async Task<ApiResponse<IReadOnlyList<ActiveSessionResponse>>> GetActiveSessionsAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var sessions = await _context.RefreshTokens.AsNoTracking()
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null && x.ExpiresAtUtc > DateTime.UtcNow)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new ActiveSessionResponse
            {
                Id = x.Id, IpAddress = x.CreatedByIp, Device = x.UserAgent ?? "Unknown device",
                CreatedAtUtc = x.CreatedAtUtc, ExpiresAtUtc = x.ExpiresAtUtc
            }).ToListAsync(cancellationToken);
        return ApiResponse<IReadOnlyList<ActiveSessionResponse>>.SuccessResponse(sessions);
    }

    public async Task<ApiResponse<bool>> RevokeSessionAsync(Guid sessionId, string ipAddress, CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var session = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Id == sessionId && x.UserId == userId, cancellationToken);
        if (session is null) return ApiResponse<bool>.FailureResponse("Session not found.");
        session.RevokedAtUtc = DateTime.UtcNow;
        session.RevokedByIp = ipAddress;
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true, "Session revoked.");
    }

    public async Task<ApiResponse<bool>> RevokeAllSessionsAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var now = DateTime.UtcNow;
        await _context.RefreshTokens.Where(x => x.UserId == userId && x.RevokedAtUtc == null && x.ExpiresAtUtc > now)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.RevokedAtUtc, now).SetProperty(x => x.RevokedByIp, ipAddress), cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true, "All active sessions revoked.");
    }

    private async Task<ApplicationUser> GetAuthenticatedUserAsync()
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        return await _userManager.FindByIdAsync(userId.ToString()) ?? throw new UnauthorizedAccessException();
    }

    private static string NormalizeCode(string code) => (code ?? string.Empty).Replace(" ", string.Empty).Replace("-", string.Empty);

    private static UserResponse MapToUserResponse(ApplicationUser user, IList<string> roles)
    {
        return new UserResponse
        {
            Id = user.Id,
            TenantId = user.TenantId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            Username = user.UserName ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            Roles = roles,
            CreatedAtUtc = user.CreatedAtUtc
        };
    }
}
