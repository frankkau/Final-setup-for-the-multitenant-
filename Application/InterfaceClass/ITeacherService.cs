using CouncilRevenueCollection.Application.DTOs.Teacher;


namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface ITeacherService
{
    Task<IEnumerable<TeacherDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<TeacherDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<TeacherDto> CreateAsync(
        CreateTeacherDto dto,
        CancellationToken cancellationToken);

    Task<TeacherDto> UpdateAsync(
        Guid id,
        UpdateTeacherDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}
