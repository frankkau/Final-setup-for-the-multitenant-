using CouncilRevenueCollection.Models.Dtos.Invoice;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface IInvoiceService
{
    Task<IReadOnlyList<InvoiceListDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<InvoiceDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<InvoiceDto> CreateAsync(
        CreateInvoiceDto dto,
        CancellationToken cancellationToken);

    Task<InvoiceDto> UpdateAsync(
        Guid id,
        UpdateInvoiceDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<InvoiceListDto>> GetByStudentIdAsync(
        Guid studentId,
        CancellationToken cancellationToken);
}