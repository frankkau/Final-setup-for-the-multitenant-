using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.ExamSubject;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Controllers;

[ApiController]
[Route("api/exam-subjects")]
public class ExamSubjectController : ControllerBase
{
    private readonly IExamSubjectService _examSubjectService;

    public ExamSubjectController(
        IExamSubjectService examSubjectService)
    {
        _examSubjectService = examSubjectService;
    }


    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<ExamSubjectListDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _examSubjectService.GetAllAsync(
            cancellationToken);

        return Ok(result);
    }


    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(ExamSubjectDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _examSubjectService.GetByIdAsync(
            id,
            cancellationToken);

        if (result is null)
            return NotFound(new
            {
                message = "Exam subject not found."
            });

        return Ok(result);
    }


    [HttpPost]
    [ProducesResponseType(
        typeof(ExamSubjectDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateExamSubjectDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _examSubjectService.CreateAsync(
            dto,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.ExamSubjectId
            },
            result);
    }


    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(ExamSubjectDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateExamSubjectDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _examSubjectService.UpdateAsync(
            id,
            dto,
            cancellationToken);

        return Ok(result);
    }


    [HttpDelete("{id:guid}")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _examSubjectService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}