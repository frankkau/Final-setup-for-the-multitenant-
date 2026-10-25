using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Dtos.Timetable;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface ITimetableService
{
    Task<IReadOnlyList<TimetableListDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<TimetableDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<TimetableDto> CreateAsync(
        CreateTimetableDto dto,
        CancellationToken cancellationToken);

    Task<TimetableDto> UpdateAsync(
        Guid id,
        UpdateTimetableDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}