using CouncilRevenueCollection.Models.Dtos.FeeStructure;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface IFeeStructureService
{
    Task<IReadOnlyList<FeeStructureListDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<FeeStructureDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<FeeStructureDto> CreateAsync(
        CreateFeeStructureDto dto,
        CancellationToken cancellationToken);

    Task<FeeStructureDto> UpdateAsync(
        Guid id,
        UpdateFeeStructureDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}