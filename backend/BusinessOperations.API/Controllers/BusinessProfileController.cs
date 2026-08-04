using BusinessOperations.API.Constants;
using BusinessOperations.API.DTOs.Businesses;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/business-profile")]
[Route("api/[controller]")]
[Authorize]
public sealed class BusinessProfileController : ControllerBase
{
    private readonly IBusinessProfileService _businessProfileService;

    public BusinessProfileController(IBusinessProfileService businessProfileService)
    {
        _businessProfileService = businessProfileService;
    }

    /// <summary>
    /// Gets the business profile settings.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<BusinessProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BusinessProfileResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var response = await _businessProfileService.GetProfileAsync(cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Updates the business profile and general settings.
    /// </summary>
    [HttpPut]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    [ProducesResponseType(typeof(ApiResponse<BusinessProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BusinessProfileResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateBusinessProfileRequest request, CancellationToken cancellationToken)
    {
        var response = await _businessProfileService.UpdateProfileAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
