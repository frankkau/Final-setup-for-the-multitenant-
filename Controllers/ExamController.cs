using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Controllers;

using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Dtos.Exam;
using Microsoft.AspNetCore.Mvc;



[ApiController]
[Route("api/exams")]
public class ExamController : ControllerBase
{
    private readonly IExamService _examService;

    public ExamController(IExamService examService)
    {
        _examService = examService;
    }


    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<ExamListDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var exams = await _examService.GetAllAsync(
            cancellationToken);

        return Ok(exams);
    }


    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(ExamDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var exam = await _examService.GetByIdAsync(
            id,
            cancellationToken);

        if (exam is null)
            return NotFound(new
            {
                message = "Exam not found."
            });

        return Ok(exam);
    }


    [HttpPost]
    [ProducesResponseType(
        typeof(ExamDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateExamDto dto,
        CancellationToken cancellationToken)
    {
        var exam = await _examService.CreateAsync(
            dto,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = exam.ExamId
            },
            exam);
    }


    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(ExamDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateExamDto dto,
        CancellationToken cancellationToken)
    {
        var exam = await _examService.UpdateAsync(
            id,
            dto,
            cancellationToken);

        return Ok(exam);
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
        await _examService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}