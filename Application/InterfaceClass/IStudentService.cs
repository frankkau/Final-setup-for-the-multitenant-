using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Dtos.Student;

namespace CouncilRevenueCollection.Application.InterfaceClass;



public interface IStudentService
{
    Task<IEnumerable<StudentDto>> GetAllAsync(
        CancellationToken cancellationToken);


    Task<StudentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);



    Task<StudentDto> CreateAsync(
         CreateStudentDto dto,
        CancellationToken cancellationToken);



    Task<StudentDto> UpdateAsync(
        Guid id,
        UpdateStudentDto dto,
        CancellationToken cancellationToken);



    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);


Task<IReadOnlyList<StudentSubjectDto>> GetStudentSubjectsAsync(
    Guid studentId,
    CancellationToken cancellationToken);

Task<IReadOnlyList<StudentExamDto>> GetStudentExamsAsync(
    Guid studentId,
    CancellationToken cancellationToken);

Task<StudentExamResultReportDto?> GetStudentExamResultsAsync(
    Guid studentId,
    Guid examId,
    CancellationToken cancellationToken);
Task<IReadOnlyList<StudentResultDto>> GetStudentResultsAsync(
    Guid studentId,
    CancellationToken cancellationToken);
    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}