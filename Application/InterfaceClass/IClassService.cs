using CouncilRevenueCollection.Application.DTOs.Class;

namespace CouncilRevenueCollection.Application.Interfaces;

public interface IClassService
{
    Task<IEnumerable<ClassDto>> GetAllAsync(
        CancellationToken cancellationToken);


    Task<ClassDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);


    Task<ClassDto> CreateAsync(
        CreateClassDto dto,
        CancellationToken cancellationToken);


    Task<ClassDto> UpdateAsync(
        Guid id,
        UpdateClassDto dto,
        CancellationToken cancellationToken);


    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);


    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}