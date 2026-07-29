using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Controllers;

using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/management/users")]
[Authorize(Roles = "TenantAdmin")]
public class UserManagementController : ControllerBase
{
    private readonly IUserManagementService _service;

    public UserManagementController(IUserManagementService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var user = await _service.GetByIdAsync(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        var user = await _service.CreateAsync(dto);

        return CreatedAtAction(nameof(Get),
            new { id = user.Id },
            user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        UpdateUserDto dto)
    {
        var user = await _service.UpdateAsync(id, dto);
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }

    
    [HttpPost("{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(
        string id,
        ResetPasswordDto dto)
    {
        await _service.ResetPasswordAsync(id, dto.NewPassword);

        return NoContent();
    }
}
