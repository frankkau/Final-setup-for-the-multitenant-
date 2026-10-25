using CouncilRevenueCollection.Models.Dtos.LibraryBookCopy;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface ILibraryBookCopyService
{
    Task<IReadOnlyList<LibraryBookCopyDto>> GetByBookAsync(
        Guid libraryBookId,
        CancellationToken cancellationToken);

    Task<LibraryBookCopyDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<LibraryBookCopyDto> CreateAsync(
        CreateLibraryBookCopyDto dto,
        CancellationToken cancellationToken);

    Task<LibraryBookCopyDto> UpdateAsync(
        Guid id,
        UpdateLibraryBookCopyDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}