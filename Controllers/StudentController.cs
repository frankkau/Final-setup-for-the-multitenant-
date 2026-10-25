using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CouncilRevenueCollection.Application.Interfaces;
using CouncilRevenueCollection.Models.Dtos.Student;
using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Application.InterfaceClass;

namespace CouncilRevenueCollection.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly ILogger<StudentsController> _logger;


    public StudentsController(
        IStudentService studentService,
        ILogger<StudentsController> logger)
    {
        _studentService = studentService;
        _logger = logger;
    }
    [HttpGet("student/{studentId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<StudentSubjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentSubjects(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var subjects = await _studentService
            .GetStudentSubjectsAsync(
                studentId,
                cancellationToken);


        return Ok(subjects);
    }


    /// <summary>
    /// Get all students
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StudentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _studentService
            .GetAllAsync(cancellationToken);


        return Ok(result);
    }




    /// <summary>
    /// Get student by Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _studentService
            .GetByIdAsync(
                id,
                cancellationToken);



        if (result is null)
            return NotFound(new
            {
                message = "Student not found."
            });



        return Ok(result);
    }




    /// <summary>
    /// Create student
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<StudentDto>> Create(
        CreateStudentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _studentService
            .CreateAsync(
                dto,
                cancellationToken);



        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.StudentId
            },
            result);
    }





    /// <summary>
    /// Update student
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<StudentDto>> Update(
        Guid id,
        UpdateStudentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _studentService
            .UpdateAsync(
                id,
                dto,
                cancellationToken);



        return Ok(result);
    }





    /// <summary>
    /// Delete student
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _studentService
            .DeleteAsync(
                id,
                cancellationToken);



        _logger.LogInformation(
            "Student {StudentId} deleted",
            id);



        return NoContent();
    }

    [HttpGet("{studentId:guid}/exams")]
    [ProducesResponseType(
        typeof(IReadOnlyList<StudentExamDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentExams(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var exams = await _studentService
            .GetStudentExamsAsync(
                studentId,
                cancellationToken);

        return Ok(exams);
    }

    [HttpGet("{studentId:guid}/results")]
    [ProducesResponseType(
        typeof(IReadOnlyList<StudentResultDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentResults(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var results = await _studentService
            .GetStudentResultsAsync(
                studentId,
                cancellationToken);

        return Ok(results);
    }

        [HttpGet("{studentId:guid}/results/{examId:guid}")]
    [ProducesResponseType(
        typeof(StudentExamResultReportDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentExamResults(
        Guid studentId,
        Guid examId,
        CancellationToken cancellationToken)
    {
        var result =
            await _studentService
                .GetStudentExamResultsAsync(
                    studentId,
                    examId,
                    cancellationToken);

        if (result is null)
            return NotFound(new
            {
                message = "Student exam results not found."
            });

        return Ok(result);
    }



    /// <summary>
    /// Check student exists
    /// </summary>
    [HttpHead("{id:guid}")]
    public async Task<IActionResult> Exists(
        Guid id,
        CancellationToken cancellationToken)
    {
        var exists = await _studentService
            .ExistsAsync(
                id,
                cancellationToken);



        return exists
            ? Ok()
            : NotFound();
    }
}