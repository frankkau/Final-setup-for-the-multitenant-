using CouncilRevenueCollection.Models.Dtos.LibraryBook;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface ILibraryBookService
{
    Task<IReadOnlyList<LibraryBookDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<LibraryBookDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<LibraryBookDto> CreateAsync(
        CreateLibraryBookDto dto,
        CancellationToken cancellationToken);

    Task<LibraryBookDto> UpdateAsync(
        Guid id,
        UpdateLibraryBookDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}