using Microsoft.AspNetCore.Mvc;
using CouncilRevenueCollection.Application.Interfaces;
using CouncilRevenueCollection.Models.Dtos.Subject;




namespace CouncilRevenueCollection.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SubjectListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var subjects = await _subjectService.GetAllAsync(cancellationToken);

        return Ok(subjects);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SubjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var subject = await _subjectService.GetByIdAsync(
            id,
            cancellationToken);

        if (subject is null)
            return NotFound();

        return Ok(subject);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SubjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
       CreateSubjectDto dto,
        CancellationToken cancellationToken)
    {
        var subject = await _subjectService.CreateAsync(
            dto,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = subject.SubjectId },
            subject);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SubjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateSubjectDto dto,
        CancellationToken cancellationToken)
    {
        var subject = await _subjectService.UpdateAsync(
            id,
            dto,
            cancellationToken);

        return Ok(subject);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _subjectService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}