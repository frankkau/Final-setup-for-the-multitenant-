namespace CouncilRevenueCollection.Application.InterfaceClass;

using CouncilRevenueCollection.Models.Dtos.ExamSubject;



public interface IExamSubjectService
{
    Task<IReadOnlyList<ExamSubjectListDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<ExamSubjectDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<ExamSubjectDto> CreateAsync(
        CreateExamSubjectDto dto,
        CancellationToken cancellationToken);

    Task<ExamSubjectDto> UpdateAsync(
        Guid id,
        UpdateExamSubjectDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}