using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection;

using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/management/tenants")]
public class TenantManagementController : ControllerBase
{
    private readonly ITenantManagementService _service;

    public TenantManagementController(ITenantManagementService service)
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
        var tenant = await _service.GetByIdAsync(id);

        if (tenant == null)
            return NotFound();

        return Ok(tenant);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTenantDto dto)
    {
        var tenant = await _service.CreateAsync(dto);

        return CreatedAtAction(nameof(Get),
            new { id = tenant.Id },
            tenant);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        UpdateTenantDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }
}