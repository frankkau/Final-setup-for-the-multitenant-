using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CouncilRevenueCollection.Models.Dtos.AcademicStructure;


namespace CouncilRevenueCollection.Application.InterfaceClass;



// namespace CouncilRevenueCollection.Application.Services.Interfaces;

public interface IAcademicStructureService
{
    Task<AcademicStructureDto> CreateAsync(
        CreateAcademicStructureDto dto,
        CancellationToken cancellationToken = default);

    Task<AcademicStructureDto?> GetByIdAsync(
        Guid academicStructureId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AcademicStructureListDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<AcademicStructureDto> UpdateAsync(
        Guid academicStructureId,
        UpdateAcademicStructureDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid academicStructureId,
        CancellationToken cancellationToken = default);
}
