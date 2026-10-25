using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.Department;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Api.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _service;

    public DepartmentController(
        IDepartmentService service)
    {
        _service = service;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var departments =
            await _service.GetAllAsync(
                cancellationToken);

        return Ok(departments);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var department =
            await _service.GetByIdAsync(
                id,
                cancellationToken);

        if (department is null)
            return NotFound();

        return Ok(department);
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreateDepartmentDto dto,
        CancellationToken cancellationToken)
    {
        var department =
            await _service.CreateAsync(
                dto,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = department.DepartmentId
            },
            department);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateDepartmentDto dto,
        CancellationToken cancellationToken)
    {
        var department =
            await _service.UpdateAsync(
                id,
                dto,
                cancellationToken);

        return Ok(department);
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}