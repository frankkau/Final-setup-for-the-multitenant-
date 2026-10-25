
using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.Registrar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RegistrarController : ControllerBase
{
    private readonly IRegistrarService _registrarService;

    public RegistrarController(
        IRegistrarService registrarService)
    {
        _registrarService = registrarService;
    }

    // =========================================================
    // CREATE
    // POST: api/Registrar
    // =========================================================

    [HttpPost]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<ActionResult<RegistrarDto>> Create(
        [FromBody] CreateRegistrarDto dto,
        CancellationToken cancellationToken)
    {
        var registrar = await _registrarService.CreateAsync(
            dto,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = registrar.RegistrarId },
            registrar);
    }

    // =========================================================
    // GET ALL
    // GET: api/Registrar
    // =========================================================

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RegistrarDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var registrars = await _registrarService.GetAllAsync(
            cancellationToken);

        return Ok(registrars);
    }

    // =========================================================
    // GET BY ID
    // GET: api/Registrar/{id}
    // =========================================================

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RegistrarDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var registrar = await _registrarService.GetByIdAsync(
            id,
            cancellationToken);

        if (registrar is null)
            return NotFound(new
            {
                message = "Registrar not found."
            });

        return Ok(registrar);
    }

    // =========================================================
    // GET BY EMPLOYEE NUMBER
    // GET: api/Registrar/employee/{employeeNo}
    // =========================================================

    [HttpGet("employee/{employeeNo}")]
    public async Task<ActionResult<RegistrarDto>> GetByEmployeeNo(
        string employeeNo,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(employeeNo))
        {
            return BadRequest(new
            {
                message = "Employee number is required."
            });
        }

        var registrar =
            await _registrarService.GetByEmployeeNoAsync(
                employeeNo,
                cancellationToken);

        if (registrar is null)
            return NotFound(new
            {
                message = "Registrar not found."
            });

        return Ok(registrar);
    }

    // =========================================================
    // UPDATE
    // PUT: api/Registrar/{id}
    // =========================================================

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<ActionResult<RegistrarDto>> Update(
        Guid id,
        [FromBody] UpdateRegistrarDto dto,
        CancellationToken cancellationToken)
    {
        var registrar = await _registrarService.UpdateAsync(
            id,
            dto,
            cancellationToken);

        return Ok(registrar);
    }

    // =========================================================
    // DELETE
    // DELETE: api/Registrar/{id}
    // =========================================================

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _registrarService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}

