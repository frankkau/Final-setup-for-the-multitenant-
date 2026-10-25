using CouncilRevenueCollection.Models.Dtos.StudentAttendance;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface IStudentAttendanceService
{
    Task<IReadOnlyList<StudentAttendanceListDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<StudentAttendanceDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<StudentAttendanceDto> CreateAsync(
        CreateStudentAttendanceDto dto,
        CancellationToken cancellationToken);

    Task<StudentAttendanceDto> UpdateAsync(
        Guid id,
        UpdateStudentAttendanceDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentAttendanceListDto>> GetByStudentAsync(
        Guid studentId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentAttendanceListDto>> GetByEnrollmentAsync(
        Guid studentEnrollmentId,
        CancellationToken cancellationToken);
}