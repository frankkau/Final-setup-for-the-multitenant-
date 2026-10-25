using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.StudentExamResult;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Controllers;

[ApiController]
[Route("api/student-exam-results")]
public class StudentExamResultController : ControllerBase
{
    private readonly IStudentExamResultService _studentExamResultService;

    public StudentExamResultController(
        IStudentExamResultService studentExamResultService)
    {
        _studentExamResultService = studentExamResultService;
    }


    // GET: api/student-exam-results
    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<StudentExamResultListDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var results =
            await _studentExamResultService.GetAllAsync(
                cancellationToken);

        return Ok(results);
    }


    // GET: api/student-exam-results/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(StudentExamResultDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result =
            await _studentExamResultService.GetByIdAsync(
                id,
                cancellationToken);

        if (result is null)
        {
            return NotFound(new
            {
                message = "Student exam result not found."
            });
        }

        return Ok(result);
    }


    // POST: api/student-exam-results
    [HttpPost]
    [ProducesResponseType(
        typeof(StudentExamResultDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateStudentExamResultDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _studentExamResultService.CreateAsync(
                dto,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.StudentExamResultId
            },
            result);
    }


    // PUT: api/student-exam-results/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(StudentExamResultDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateStudentExamResultDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _studentExamResultService.UpdateAsync(
                id,
                dto,
                cancellationToken);

        return Ok(result);
    }


    // DELETE: api/student-exam-results/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _studentExamResultService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}