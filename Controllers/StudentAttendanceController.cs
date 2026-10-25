using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.StudentAttendance;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Controllers;

[ApiController]
[Route("api/student-attendance")]
public class StudentAttendanceController : ControllerBase
{
    private readonly IStudentAttendanceService _attendanceService;

    public StudentAttendanceController(
        IStudentAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }


    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<StudentAttendanceListDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var attendance =
            await _attendanceService.GetAllAsync(
                cancellationToken);

        return Ok(attendance);
    }


    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(StudentAttendanceDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var attendance =
            await _attendanceService.GetByIdAsync(
                id,
                cancellationToken);

        if (attendance is null)
            return NotFound(new
            {
                message = "Attendance record not found."
            });

        return Ok(attendance);
    }


    [HttpPost]
    [ProducesResponseType(
        typeof(StudentAttendanceDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateStudentAttendanceDto dto,
        CancellationToken cancellationToken)
    {
        var attendance =
            await _attendanceService.CreateAsync(
                dto,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = attendance.StudentAttendanceId
            },
            attendance);
    }


    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(StudentAttendanceDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateStudentAttendanceDto dto,
        CancellationToken cancellationToken)
    {
        var attendance =
            await _attendanceService.UpdateAsync(
                id,
                dto,
                cancellationToken);

        return Ok(attendance);
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
        await _attendanceService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }


    [HttpGet("student/{studentId:guid}")]
    [ProducesResponseType(
        typeof(IReadOnlyList<StudentAttendanceListDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByStudent(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var attendance =
            await _attendanceService.GetByStudentAsync(
                studentId,
                cancellationToken);

        return Ok(attendance);
    }


    [HttpGet("enrollment/{studentEnrollmentId:guid}")]
    [ProducesResponseType(
        typeof(IReadOnlyList<StudentAttendanceListDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEnrollment(
        Guid studentEnrollmentId,
        CancellationToken cancellationToken)
    {
        var attendance =
            await _attendanceService.GetByEnrollmentAsync(
                studentEnrollmentId,
                cancellationToken);

        return Ok(attendance);
    }
}