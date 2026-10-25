using CouncilRevenueCollection.Application.DTOs.Class;
using CouncilRevenueCollection.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CouncilRevenueCollection.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClassesController : ControllerBase
{
    private readonly IClassService _classService;
    private readonly ILogger<ClassesController> _logger;


    public ClassesController(
        IClassService classService,
        ILogger<ClassesController> logger)
    {
        _classService = classService;
        _logger = logger;
    }



    /// <summary>
    /// Get all classes for current tenant
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClassDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var classes = await _classService
            .GetAllAsync(cancellationToken);


        return Ok(classes);
    }




    /// <summary>
    /// Get class by Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClassDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClassDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _classService
            .GetByIdAsync(
                id,
                cancellationToken);



        if (result is null)
        {
            return NotFound(new
            {
                message = "Class not found."
            });
        }


        return Ok(result);
    }





    /// <summary>
    /// Create new class
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClassDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClassDto>> Create(
        [FromBody] CreateClassDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _classService
            .CreateAsync(
                dto,
                cancellationToken);



        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.ClassId
            },
            result);
    }





    /// <summary>
    /// Update class
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ClassDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClassDto>> Update(
        Guid id,
        [FromBody] UpdateClassDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _classService
            .UpdateAsync(
                id,
                dto,
                cancellationToken);



        return Ok(result);
    }





    /// <summary>
    /// Delete class
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _classService
            .DeleteAsync(
                id,
                cancellationToken);



        _logger.LogInformation(
            "Class {ClassId} deleted successfully",
            id);



        return NoContent();
    }





    /// <summary>
    /// Check if class exists
    /// </summary>
    [HttpHead("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Exists(
        Guid id,
        CancellationToken cancellationToken)
    {
        var exists = await _classService
            .ExistsAsync(
                id,
                cancellationToken);



        if (!exists)
            return NotFound();



        return Ok();
    }
}