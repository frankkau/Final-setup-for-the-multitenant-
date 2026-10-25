using CouncilRevenueCollection.Application.DTOs.Teacher;
using CouncilRevenueCollection.Application.InterfaceClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CouncilRevenueCollection.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _teacherService;
    private readonly ILogger<TeachersController> _logger;


    public TeachersController(
        ITeacherService teacherService,
        ILogger<TeachersController> logger)
    {
        _teacherService = teacherService;
        _logger = logger;
    }



    /// <summary>
    /// Get all teachers for current tenant
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TeacherDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TeacherDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var teachers = await _teacherService
            .GetAllAsync(cancellationToken);

        return Ok(teachers);
    }




    /// <summary>
    /// Get teacher by id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TeacherDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeacherDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var teacher = await _teacherService
            .GetByIdAsync(id, cancellationToken);


        if (teacher is null)
            return NotFound(new
            {
                message = "Teacher not found."
            });


        return Ok(teacher);
    }




    /// <summary>
    /// Create teacher
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TeacherDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TeacherDto>> Create(
        [FromBody] CreateTeacherDto dto,
        CancellationToken cancellationToken)
    {
        var teacher = await _teacherService
            .CreateAsync(dto, cancellationToken);



        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = teacher.Id
            },
            teacher);
    }





    /// <summary>
    /// Update teacher
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TeacherDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeacherDto>> Update(
        Guid id,
        [FromBody] UpdateTeacherDto dto,
        CancellationToken cancellationToken)
    {
        var teacher = await _teacherService
            .UpdateAsync(
                id,
                dto,
                cancellationToken);



        return Ok(teacher);
    }





    /// <summary>
    /// Delete teacher
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _teacherService
            .DeleteAsync(
                id,
                cancellationToken);



        _logger.LogInformation(
            "Teacher {Id} deleted successfully",
            id);


        return NoContent();
    }




    /// <summary>
    /// Check if teacher exists
    /// </summary>
    [HttpHead("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Exists(
        Guid id,
        CancellationToken cancellationToken)
    {
        var exists = await _teacherService
            .ExistsAsync(
                id,
                cancellationToken);


        if (!exists)
            return NotFound();


        return Ok();
    }
}