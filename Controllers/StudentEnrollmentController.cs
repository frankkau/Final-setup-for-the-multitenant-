using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Dtos.StudentEnrollment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CouncilRevenueCollection.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentEnrollmentsController : ControllerBase
{
    private readonly IStudentEnrollmentService _service;
    private readonly ILogger<StudentEnrollmentsController> _logger;


    public StudentEnrollmentsController(
        IStudentEnrollmentService service,
        ILogger<StudentEnrollmentsController> logger)
    {
        _service = service;
        _logger = logger;
    }



    // GET: api/StudentEnrollments
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StudentEnrollmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StudentEnrollmentDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _service
            .GetAllAsync(cancellationToken);

        return Ok(result);
    }





    // GET: api/StudentEnrollments/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(StudentEnrollmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentEnrollmentDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _service
            .GetByIdAsync(
                id,
                cancellationToken);


        if (result is null)
        {
            return NotFound(new
            {
                message = "Student enrollment not found."
            });
        }


        return Ok(result);
    }





    // POST: api/StudentEnrollments
    [HttpPost]
    [ProducesResponseType(typeof(StudentEnrollmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StudentEnrollmentDto>> Create(
        [FromBody] CreateStudentEnrollmentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service
            .CreateAsync(
                dto,
                cancellationToken);



        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.StudentEnrollmentId
            },
            result);
    }





    // PUT: api/StudentEnrollments/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(StudentEnrollmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentEnrollmentDto>> Update(
        Guid id,
        [FromBody] UpdateStudentEnrollmentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service
            .UpdateAsync(
                id,
                dto,
                cancellationToken);



        return Ok(result);
    }





    // DELETE: api/StudentEnrollments/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _service
            .DeleteAsync(
                id,
                cancellationToken);



        _logger.LogInformation(
            "Student enrollment {EnrollmentId} deleted",
            id);



        return NoContent();
    }





    // HEAD: api/StudentEnrollments/{id}
    [HttpHead("{id:guid}")]
    public async Task<IActionResult> Exists(
        Guid id,
        CancellationToken cancellationToken)
    {
        var exists = await _service
            .ExistsAsync(
                id,
                cancellationToken);



        if (!exists)
            return NotFound();


        return Ok();
    }
}