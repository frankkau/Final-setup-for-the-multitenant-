using CouncilRevenueCollection.Application.DTOs.ClassSubject;
using CouncilRevenueCollection.Application.Models.Dtos.ClassSubject;
using CouncilRevenueCollection.Models.Dtos.ClassSubject;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface IClassSubjectService
{
    Task<IReadOnlyList<ClassSubjectListDto>> GetAllAsync(
        CancellationToken cancellationToken);


    Task<ClassSubjectDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);


    Task<ClassSubjectDto> CreateAsync(
        CreateClassSubjectDto dto,
        CancellationToken cancellationToken);


    Task<ClassSubjectDto> UpdateAsync(
        Guid id,
        UpdateClassSubjectDto dto,
        CancellationToken cancellationToken);


    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);


    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}