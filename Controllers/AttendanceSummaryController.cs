using CouncilRevenueCollection.Application.InterfaceClass;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Controllers;

[ApiController]
[Route("api/attendance-summary")]
public class AttendanceSummaryController : ControllerBase
{
    private readonly IAttendanceSummaryService _summaryService;

    public AttendanceSummaryController(
        IAttendanceSummaryService summaryService)
    {
        _summaryService = summaryService;
    }


    [HttpGet("student/{studentId:guid}")]
    public async Task<IActionResult> GetByStudent(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var summary =
            await _summaryService.GetByStudentAsync(
                studentId,
                cancellationToken);

        if (summary is null)
            return NotFound(new
            {
                message = "Student enrollment not found."
            });

        return Ok(summary);
    }


    [HttpGet("enrollment/{studentEnrollmentId:guid}")]
    public async Task<IActionResult> GetByEnrollment(
        Guid studentEnrollmentId,
        CancellationToken cancellationToken)
    {
        var summary =
            await _summaryService.GetByEnrollmentAsync(
                studentEnrollmentId,
                cancellationToken);

        if (summary is null)
            return NotFound(new
            {
                message = "Student enrollment not found."
            });

        return Ok(summary);
    }
}