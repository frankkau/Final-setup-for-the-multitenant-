using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.AcademicStructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Api.Controllers;

[ApiController]
[Route("api/academic-structures")]
[Authorize]
public class AcademicStructuresController : ControllerBase
{
    private readonly IAcademicStructureService _academicStructureService;
    private readonly ILogger<AcademicStructuresController> _logger;


    public AcademicStructuresController(
        IAcademicStructureService academicStructureService,
        ILogger<AcademicStructuresController> logger)
    {
        _academicStructureService = academicStructureService;
        _logger = logger;
    }
    /// <summary>
    /// Create a new academic structure
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AcademicStructureDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AcademicStructureDto>> Create(
        [FromBody] CreateAcademicStructureDto dto,
        CancellationToken cancellationToken)
    {

        var result = await _academicStructureService
            .CreateAsync(dto, cancellationToken);


        return CreatedAtAction(
            nameof(GetById),
            new 
            { 
                id = result.AcademicStructureId 
            },
            result);
    }
    /// <summary>
    /// Get academic structure by id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AcademicStructureDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AcademicStructureDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {

        var result = await _academicStructureService
            .GetByIdAsync(id, cancellationToken);


        if (result is null)
            return NotFound(
                new 
                {
                    message = "Academic structure not found."
                });


        return Ok(result);
    }
    /// <summary>
    /// Get all academic structures for current tenant
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AcademicStructureListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AcademicStructureListDto>>> GetAll(
        CancellationToken cancellationToken)
    {

        var result = await _academicStructureService
            .GetAllAsync(cancellationToken);


        return Ok(result);
    }
    /// <summary>
    /// Update academic structure
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AcademicStructureDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AcademicStructureDto>> Update(
        Guid id,
        [FromBody] UpdateAcademicStructureDto dto,
        CancellationToken cancellationToken)
    {

        var result = await _academicStructureService
            .UpdateAsync(id, dto, cancellationToken);


        return Ok(result);
    }
    /// <summary>
    /// Delete academic structure
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {

        await _academicStructureService
            .DeleteAsync(id, cancellationToken);


        return NoContent();
    }
}