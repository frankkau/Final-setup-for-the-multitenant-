using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Dtos.Exam;

namespace CouncilRevenueCollection.Application.InterfaceClass;



public interface IExamService
{
    Task<IReadOnlyList<ExamListDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<ExamDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<ExamDto> CreateAsync(
        CreateExamDto dto,
        CancellationToken cancellationToken);

    Task<ExamDto> UpdateAsync(
        Guid id,
        UpdateExamDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}