using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.TeacherSubject;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Controllers;

[ApiController]
[Route("api/teacher-subjects")]
public class TeacherSubjectController : ControllerBase
{
    private readonly ITeacherSubjectService _teacherSubjectService;

    public TeacherSubjectController(
        ITeacherSubjectService teacherSubjectService)
    {
        _teacherSubjectService = teacherSubjectService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<TeacherSubjectListDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _teacherSubjectService
            .GetAllAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(TeacherSubjectDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _teacherSubjectService
            .GetByIdAsync(
                id,
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(TeacherSubjectDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateTeacherSubjectDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _teacherSubjectService
            .CreateAsync(
                dto,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.TeacherSubjectId },
            result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(TeacherSubjectDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateTeacherSubjectDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _teacherSubjectService
            .UpdateAsync(
                id,
                dto,
                cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _teacherSubjectService
            .DeleteAsync(
                id,
                cancellationToken);

        return NoContent();
    }

    [HttpGet("exists/{id:guid}")]
    [ProducesResponseType(
        typeof(bool),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> Exists(
        Guid id,
        CancellationToken cancellationToken)
    {
        var exists = await _teacherSubjectService
            .ExistsAsync(
                id,
                cancellationToken);

        return Ok(exists);
    }
}