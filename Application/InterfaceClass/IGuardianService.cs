using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Dtos.Guardian;

namespace CouncilRevenueCollection.Application.Implementation;



public interface IGuardianService
{
    Task<IEnumerable<GuardianDto>> GetAllAsync(
        CancellationToken cancellationToken);


    Task<GuardianDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);



    Task<GuardianDto> CreateAsync(
        CreateGuardianDto dto,
        CancellationToken cancellationToken);



    Task<GuardianDto> UpdateAsync(
        Guid id,
        UpdateGuardianDto dto,
        CancellationToken cancellationToken);



    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);



    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}