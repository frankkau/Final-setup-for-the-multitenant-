namespace CouncilRevenueCollection.Application.InterfaceClass;

using CouncilRevenueCollection.Models.Dtos.StudentAttendance;


public interface IAttendanceSummaryService
{
    Task<AttendanceSummaryDto?> GetByStudentAsync(
        Guid studentId,
        CancellationToken cancellationToken);

    Task<AttendanceSummaryDto?> GetByEnrollmentAsync(
        Guid studentEnrollmentId,
        CancellationToken cancellationToken);
}