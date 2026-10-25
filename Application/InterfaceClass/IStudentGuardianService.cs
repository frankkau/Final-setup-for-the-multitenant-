using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Dtos.StudentGuardian;

namespace CouncilRevenueCollection.Application.InterfaceClass;
public interface IStudentGuardianService
{
    Task<IEnumerable<StudentGuardianDto>> GetAllAsync(
        CancellationToken cancellationToken);



    Task<StudentGuardianDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);



    Task<StudentGuardianDto> CreateAsync(
        CreateStudentGuardianDto dto,
        CancellationToken cancellationToken);



    Task<StudentGuardianDto> UpdateAsync(
        Guid id,
        UpdateStudentGuardianDto dto,
        CancellationToken cancellationToken);



    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);



    Task<IEnumerable<StudentGuardianDto>> GetStudentGuardiansAsync(
        Guid studentId,
        CancellationToken cancellationToken);



    Task<IEnumerable<StudentGuardianDto>> GetGuardianStudentsAsync(
        Guid guardianId,
        CancellationToken cancellationToken);
}