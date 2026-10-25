using CouncilRevenueCollection.Models.Dtos.FormGrade;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface IFormGradeService
{
    Task<FormGradeDto> CreateAsync(
        CreateFormGradeDto dto,
        CancellationToken cancellationToken = default);

    Task<FormGradeDto?> GetByIdAsync(
        Guid formGradeId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FormGradeListDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<FormGradeDto> UpdateAsync(
        Guid formGradeId,
        UpdateFormGradeDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid formGradeId,
        CancellationToken cancellationToken = default);
}