
using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.StudentGuardian;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CouncilRevenueCollection.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentGuardiansController : ControllerBase
{
    private readonly IStudentGuardianService _service;
    private readonly ILogger<StudentGuardiansController> _logger;


    public StudentGuardiansController(
        IStudentGuardianService service,
        ILogger<StudentGuardiansController> logger)
    {
        _service = service;
        _logger = logger;
    }



    // GET: api/StudentGuardians
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StudentGuardianDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StudentGuardianDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _service
            .GetAllAsync(cancellationToken);


        return Ok(result);
    }





    // GET: api/StudentGuardians/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(StudentGuardianDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentGuardianDto>> GetById(
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
                message = "Student guardian relationship not found."
            });
        }



        return Ok(result);
    }





    // POST: api/StudentGuardians
    [HttpPost]
    [ProducesResponseType(typeof(StudentGuardianDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StudentGuardianDto>> Create(
        [FromBody] CreateStudentGuardianDto dto,
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
                id = result.StudentGuardianId
            },
            result);
    }





    // PUT: api/StudentGuardians/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(StudentGuardianDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentGuardianDto>> Update(
        Guid id,
        [FromBody] UpdateStudentGuardianDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service
            .UpdateAsync(
                id,
                dto,
                cancellationToken);



        return Ok(result);
    }





    // DELETE: api/StudentGuardians/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _service
            .DeleteAsync(
                id,
                cancellationToken);



        _logger.LogInformation(
            "Student guardian relationship {Id} deleted",
            id);



        return NoContent();
    }





    // GET: api/StudentGuardians/student/{studentId}
    // Get all guardians belonging to a student
    [HttpGet("student/{studentId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<StudentGuardianDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StudentGuardianDto>>> GetStudentGuardians(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var result = await _service
            .GetStudentGuardiansAsync(
                studentId,
                cancellationToken);



        return Ok(result);
    }





    // GET: api/StudentGuardians/guardian/{guardianId}
    // Get all students belonging to a guardian
    [HttpGet("guardian/{guardianId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<StudentGuardianDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StudentGuardianDto>>> GetGuardianStudents(
        Guid guardianId,
        CancellationToken cancellationToken)
    {
        var result = await _service
            .GetGuardianStudentsAsync(
                guardianId,
                cancellationToken);



        return Ok(result);
    }
}