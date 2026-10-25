using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Dtos.TeacherSubject;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface ITeacherSubjectService
{
    Task<IReadOnlyList<TeacherSubjectListDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<TeacherSubjectDto?> GetByIdAsync(
        Guid teacherSubjectId,
        CancellationToken cancellationToken);

    Task<TeacherSubjectDto> CreateAsync(
        CreateTeacherSubjectDto dto,
        CancellationToken cancellationToken);

    Task<TeacherSubjectDto> UpdateAsync(
        Guid teacherSubjectId,
        UpdateTeacherSubjectDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid teacherSubjectId,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid teacherSubjectId,
        CancellationToken cancellationToken);
}