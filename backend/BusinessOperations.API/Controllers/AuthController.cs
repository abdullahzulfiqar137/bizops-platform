using BusinessOperations.API.DTOs.Authentication;
using BusinessOperations.API.DTOs.Users;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private const string RefreshCookieName = "bo_refresh";
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    [HttpPost("register")]
    [EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await _authenticationService.RegisterAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Authenticates a user and generates JWT access and refresh tokens.
    /// </summary>
    [HttpPost("login")]
    [EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status412PreconditionFailed)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var ipAddress = GetClientIpAddress();
        var response = await _authenticationService.LoginAsync(request, ipAddress, Request.Headers.UserAgent.ToString(), cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        SetRefreshCookie(response.Data!);

        return Ok(response);
    }

    /// <summary>
    /// Refreshes expired JWT access token using a valid refresh token.
    /// </summary>
    [HttpPost("refresh-token")]
    [EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(RefreshCookieName, out var refreshToken))
            return BadRequest(ApiResponse<AuthResponse>.FailureResponse("Refresh session cookie is missing."));
        request.RefreshToken = refreshToken;
        var ipAddress = GetClientIpAddress();
        var response = await _authenticationService.RefreshTokenAsync(request, ipAddress, Request.Headers.UserAgent.ToString(), cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        SetRefreshCookie(response.Data!);

        return Ok(response);
    }

    /// <summary>
    /// Revokes an active refresh token.
    /// </summary>
    [HttpPost("revoke-token")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request, CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(RefreshCookieName, out var refreshToken))
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Session is already signed out."));
        request.RefreshToken = refreshToken;
        var ipAddress = GetClientIpAddress();
        var response = await _authenticationService.RevokeTokenAsync(request, ipAddress, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        Response.Cookies.Delete(RefreshCookieName, RefreshCookieOptions(DateTimeOffset.UtcNow.AddDays(-1)));
        return Ok(response);
    }

    private void SetRefreshCookie(AuthResponse response)
    {
        Response.Cookies.Append(
            RefreshCookieName,
            response.RefreshToken,
            RefreshCookieOptions(new DateTimeOffset(response.RefreshTokenExpiresAtUtc, TimeSpan.Zero)));
    }

    private CookieOptions RefreshCookieOptions(DateTimeOffset expires) => new()
    {
        HttpOnly = true,
        Secure = Request.IsHttps,
        SameSite = SameSiteMode.Strict,
        Path = "/api",
        Expires = expires,
        IsEssential = true
    };

    /// <summary>
    /// Sends password recovery instructions without revealing whether an account exists.
    /// </summary>
    [HttpPost("forgot-password")]
    [EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _authenticationService.ForgotPasswordAsync(
            request,
            cancellationToken);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Resets a password using a single-use ASP.NET Identity token.
    /// </summary>
    [HttpPost("reset-password")]
    [EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _authenticationService.ResetPasswordAsync(
            request,
            cancellationToken);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Retrieves current authenticated user profile details.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var response = await _authenticationService.GetCurrentUserProfileAsync(cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateCurrentUser(
        [FromBody] UpdateMyProfileRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _authenticationService.UpdateCurrentUserProfileAsync(request, cancellationToken);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("security/two-factor")]
    [Authorize]
    public async Task<IActionResult> TwoFactorSetup(CancellationToken token) => Ok(await _authenticationService.GetTwoFactorSetupAsync(token));

    [HttpPost("security/two-factor/enable")]
    [Authorize]
    public async Task<IActionResult> EnableTwoFactor(VerifyTwoFactorRequest request, CancellationToken token)
    {
        var response = await _authenticationService.EnableTwoFactorAsync(request, token);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("security/two-factor/disable")]
    [Authorize]
    public async Task<IActionResult> DisableTwoFactor(VerifyTwoFactorRequest request, CancellationToken token)
    {
        var response = await _authenticationService.DisableTwoFactorAsync(request, token);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("security/sessions")]
    [Authorize]
    public async Task<IActionResult> Sessions(CancellationToken token) => Ok(await _authenticationService.GetActiveSessionsAsync(token));

    [HttpDelete("security/sessions/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> RevokeSession(Guid id, CancellationToken token)
    {
        var response = await _authenticationService.RevokeSessionAsync(id, GetClientIpAddress(), token);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpDelete("security/sessions")]
    [Authorize]
    public async Task<IActionResult> RevokeAllSessions(CancellationToken token) =>
        Ok(await _authenticationService.RevokeAllSessionsAsync(GetClientIpAddress(), token));

    private string GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
    }
}
