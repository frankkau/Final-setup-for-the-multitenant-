using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Controllers;

[ApiController]
[Route("api/management/roles")]
// [Authorize]
public class RoleManagementController : ControllerBase
{
    private readonly IRoleManagementService _service;

    public RoleManagementController(IRoleManagementService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
    {
        await _service.CreateAsync(dto);

        return Ok(new
        {
            message = "Role created successfully."
        });
    }

    [HttpDelete("{roleName}")]
    public async Task<IActionResult> Delete(string roleName)
    {
        await _service.DeleteAsync(roleName);

        return NoContent();
    }

    [HttpPost("{roleName}/users/{userId}")]
    public async Task<IActionResult> AssignUser(
        string roleName,
        string userId)
    {
        await _service.AssignUserAsync(userId, roleName);

        return Ok(new
        {
            message = "Role assigned successfully."
        });
    }

    [HttpDelete("{roleName}/users/{userId}")]
    public async Task<IActionResult> RemoveUser(
        string roleName,
        string userId)
    {
        await _service.RemoveUserAsync(userId, roleName);

        return NoContent();
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        return Ok(await _service.GetUserRolesAsync(userId));
    }
}