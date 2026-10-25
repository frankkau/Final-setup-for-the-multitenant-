using CouncilRevenueCollection.Models.Dtos.FeePayment;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface IFeePaymentService
{
    Task<IReadOnlyList<FeePaymentListDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<FeePaymentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<FeePaymentDto> CreateAsync(
        CreateFeePaymentDto dto,
        CancellationToken cancellationToken);

    Task<FeePaymentDto> UpdateAsync(
        Guid id,
        UpdateFeePaymentDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}