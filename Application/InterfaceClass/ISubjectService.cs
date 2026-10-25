using CouncilRevenueCollection.Models.Dtos.Subject;

namespace CouncilRevenueCollection.Application.Interfaces;

public interface ISubjectService
{
    Task<IReadOnlyList<SubjectListDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<SubjectDto?> GetByIdAsync(
        Guid subjectId,
        CancellationToken cancellationToken);

    Task<SubjectDto> CreateAsync(
        CreateSubjectDto dto,
        CancellationToken cancellationToken);

    Task<SubjectDto> UpdateAsync(
        Guid subjectId,
        UpdateSubjectDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid subjectId,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid subjectId,
        CancellationToken cancellationToken);
}