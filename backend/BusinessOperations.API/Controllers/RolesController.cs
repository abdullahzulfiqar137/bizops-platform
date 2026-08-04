using BusinessOperations.API.Constants;
using BusinessOperations.API.DTOs.Users;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public sealed class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Gets all system roles.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
    {
        var response = await _roleService.GetAllRolesAsync(cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Assigns role(s) to a user account.
    /// </summary>
    [HttpPost("assign")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignRoles([FromBody] AssignRoleRequest request, CancellationToken cancellationToken)
    {
        var response = await _roleService.AssignRolesToUserAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
