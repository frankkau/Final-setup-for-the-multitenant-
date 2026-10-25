using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Dtos.StudentEnrollment;

namespace CouncilRevenueCollection.Application.InterfaceClass;




public interface IStudentEnrollmentService
{
    Task<IEnumerable<StudentEnrollmentDto>> GetAllAsync(
        CancellationToken cancellationToken);


    Task<StudentEnrollmentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);



    Task<StudentEnrollmentDto> CreateAsync(
        CreateStudentEnrollmentDto dto,
        CancellationToken cancellationToken);



    Task<StudentEnrollmentDto> UpdateAsync(
        Guid id,
        UpdateStudentEnrollmentDto dto,
        CancellationToken cancellationToken);



    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);



    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}