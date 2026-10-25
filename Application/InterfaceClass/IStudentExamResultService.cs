using CouncilRevenueCollection.Models.Dtos.StudentExamResult;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface IStudentExamResultService
{
    Task<IReadOnlyList<StudentExamResultListDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<StudentExamResultDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<StudentExamResultDto> CreateAsync(
        CreateStudentExamResultDto dto,
        CancellationToken cancellationToken);

    Task<StudentExamResultDto> UpdateAsync(
        Guid id,
        UpdateStudentExamResultDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}