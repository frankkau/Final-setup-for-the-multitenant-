using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Domain.Entities;
using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Dtos.Student;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Infrastructure.Services;

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<StudentService> _logger;

    public StudentService(
        ApplicationDbContext context,
        ITenantService tenantService,
        UserManager<User> userManager,
        ILogger<StudentService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<StudentDto> CreateAsync(
        CreateStudentDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var studentNo = dto.StudentNo.Trim();
        var email = dto.Email.Trim().ToLowerInvariant();
        var firstName = dto.FirstName.Trim();
        var lastName = dto.LastName.Trim();
        var phone = dto.Phone.Trim();
        var address = dto.Address.Trim();

        // Student number must be unique within tenant.
        var studentExists = await _context.Students
            .AnyAsync(
                x =>
                    x.TenantId == tenantId &&
                    x.StudentNo == studentNo,
                cancellationToken);

        if (studentExists)
        {
            throw new InvalidOperationException(
                "Student number already exists.");
        }

        // Student email must be unique within tenant.
        var studentEmailExists = await _context.Students
            .AnyAsync(
                x =>
                    x.TenantId == tenantId &&
                    x.Email == email,
                cancellationToken);

        if (studentEmailExists)
        {
            throw new InvalidOperationException(
                "Email already belongs to another student.");
        }

        // Email must also be unique among Identity users in this tenant.
        var userEmailExists = await _userManager.Users
            .AnyAsync(
                x =>
                    x.TenantId == tenantId &&
                    x.Email == email,
                cancellationToken);

        if (userEmailExists)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            // -----------------------------------------
            // 1. Create Identity User
            // -----------------------------------------

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),

                TenantId = tenantId,

                UserName = email,

                Email = email,

                FirstName = firstName,

                LastName = lastName
            };

            var userResult = await _userManager.CreateAsync(
                user,
                dto.Password);

            if (!userResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    userResult.Errors.Select(x => x.Description));

                throw new InvalidOperationException(
                    $"Unable to create student account: {errors}");
            }

            // -----------------------------------------
            // 2. Assign Student Role
            // -----------------------------------------

            var roleResult = await _userManager.AddToRoleAsync(
                user,
                "Student");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(x => x.Description));

                throw new InvalidOperationException(
                    $"Unable to assign Student role: {errors}");
            }

            // -----------------------------------------
            // 3. Create Student
            // -----------------------------------------

            var student = new Student
            {
                StudentId = Guid.NewGuid(),

                TenantId = tenantId,

                UserId = user.Id,

                StudentNo = studentNo,

                FirstName = firstName,

                LastName = lastName,

                DateOfBirth = dto.DateOfBirth,

                Gender = dto.Gender.Trim(),

                Phone = phone,

                Email = email,

                Address = address,

                // GuardianName = dto.GuardianName.Trim(),

                // GuardianPhone = dto.GuardianPhone.Trim(),

                Status = dto.Status,

                CreatedAt = DateTime.UtcNow
            };

            await _context.Students.AddAsync(
                student,
                cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            _logger.LogInformation(
                "Student {StudentId} created with User {UserId}",
                student.StudentId,
                user.Id);

            return Map(student);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    public async Task<IReadOnlyList<StudentSubjectDto>>
        GetStudentSubjectsAsync(
            Guid studentId,
            CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var studentExists = await _context.Students
            .AnyAsync(
                x =>
                    x.StudentId == studentId &&
                    x.TenantId == tenantId,
                cancellationToken);

        if (!studentExists)
        {
            throw new KeyNotFoundException(
                "Student not found.");
        }

        // Get the student's latest active enrollment.
        var classId = await _context.StudentEnrollments
            .AsNoTracking()
            .Where(
                x =>
                    x.StudentId == studentId &&
                    x.TenantId == tenantId &&
                    x.Status)
            .OrderByDescending(x => x.EnrollmentDate)
            .Select(x => (Guid?)x.ClassId)
            .FirstOrDefaultAsync(
                cancellationToken);

        if (classId is null)
        {
            return Array.Empty<StudentSubjectDto>();
        }

        return await _context.ClassSubjects
            .AsNoTracking()
            .Where(
                x =>
                    x.TenantId == tenantId &&
                    x.ClassId == classId.Value)
            .Select(x => new StudentSubjectDto
            {
                SubjectId = x.SubjectId,

                SubjectName = x.Subject.SubjectName,

                PeriodsPerWeek = x.PeriodsPerWeek,

                IsOptional = x.IsOptional
            })
            .OrderBy(x => x.SubjectName)
            .ToListAsync(cancellationToken);
    }

    public async Task<StudentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.Students
            .AsNoTracking()
            .Where(
                x =>
                    x.StudentId == id &&
                    x.TenantId == tenantId)
            .Select(x => new StudentDto
            {
                StudentId = x.StudentId,

                UserId = x.UserId,

                StudentNo = x.StudentNo,

                FirstName = x.FirstName,

                LastName = x.LastName,

                DateOfBirth = x.DateOfBirth,

                Gender = x.Gender,

                Phone = x.Phone,

                Email = x.Email,

                Address = x.Address,

                // GuardianName = x.GuardianName,

                // GuardianPhone = x.GuardianPhone,

                Status = x.Status
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<StudentDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.Students
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .Select(x => new StudentDto
            {
                StudentId = x.StudentId,

                UserId = x.UserId,

                StudentNo = x.StudentNo,

                FirstName = x.FirstName,

                LastName = x.LastName,

                DateOfBirth = x.DateOfBirth,

                Gender = x.Gender,

                Phone = x.Phone,

                Email = x.Email,

                Address = x.Address,

                // GuardianName = x.GuardianName,

                // GuardianPhone = x.GuardianPhone,

                Status = x.Status
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<StudentDto> UpdateAsync(
        Guid id,
        UpdateStudentDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var student = await _context.Students
            .FirstOrDefaultAsync(
                x =>
                    x.StudentId == id &&
                    x.TenantId == tenantId,
                cancellationToken);

        if (student is null)
        {
            throw new KeyNotFoundException(
                "Student not found.");
        }

        var email = dto.Email.Trim().ToLowerInvariant();

        // Check whether another student uses this email.
        var emailExists = await _context.Students
            .AnyAsync(
                x =>
                    x.StudentId != id &&
                    x.TenantId == tenantId &&
                    x.Email == email,
                cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "Email already belongs to another student.");
        }

        // Load Identity user.
        var user = await _userManager.FindByIdAsync(
            student.UserId);

        if (user is null)
        {
            throw new InvalidOperationException(
                "Student's user account could not be found.");
        }

        // -----------------------------------------
        // Update Student
        // -----------------------------------------

        student.FirstName = dto.FirstName.Trim();

        student.LastName = dto.LastName.Trim();

        student.DateOfBirth = dto.DateOfBirth;

        student.Gender = dto.Gender.Trim();

        student.Phone = dto.Phone.Trim();

        student.Email = email;

        student.Address = dto.Address.Trim();

        // student.GuardianName = dto.GuardianName.Trim();

        // student.GuardianPhone = dto.GuardianPhone.Trim();

        student.Status = dto.Status;

        // -----------------------------------------
        // Update Identity User
        // -----------------------------------------

        user.FirstName = student.FirstName;

        user.LastName = student.LastName;

        user.Email = email;

        user.UserName = email;

        var userResult = await _userManager.UpdateAsync(user);

        if (!userResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                userResult.Errors.Select(x => x.Description));

            throw new InvalidOperationException(
                $"Unable to update student account: {errors}");
        }

        await _context.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Student {StudentId} updated",
            id);

        return Map(student);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var student = await _context.Students
            .FirstOrDefaultAsync(
                x =>
                    x.StudentId == id &&
                    x.TenantId == tenantId,
                cancellationToken);

        if (student is null)
        {
            throw new KeyNotFoundException(
                "Student not found.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var user = await _userManager.FindByIdAsync(
                student.UserId);

            // Delete Student first.
            _context.Students.Remove(student);

            await _context.SaveChangesAsync(
                cancellationToken);

            // Then delete Identity User.
            if (user is not null)
            {
                var userResult =
                    await _userManager.DeleteAsync(user);

                if (!userResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        userResult.Errors.Select(
                            x => x.Description));

                    throw new InvalidOperationException(
                        $"Unable to delete student account: {errors}");
                }
            }

            await transaction.CommitAsync(
                cancellationToken);

            _logger.LogInformation(
                "Student {StudentId} and User {UserId} deleted",
                id,
                student.UserId);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    public async Task<IReadOnlyList<StudentExamDto>>
        GetStudentExamsAsync(
            Guid studentId,
            CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var studentExists = await _context.Students
            .AnyAsync(
                x =>
                    x.StudentId == studentId &&
                    x.TenantId == tenantId,
                cancellationToken);

        if (!studentExists)
        {
            throw new KeyNotFoundException(
                "Student not found.");
        }

        var classIds = await _context.StudentEnrollments
            .AsNoTracking()
            .Where(
                x =>
                    x.StudentId == studentId &&
                    x.TenantId == tenantId &&
                    x.Status)
            .Select(x => x.ClassId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (classIds.Count == 0)
        {
            return Array.Empty<StudentExamDto>();
        }

        return await _context.Exams
            .AsNoTracking()
            .Where(
                exam =>
                    exam.TenantId == tenantId &&
                    exam.ExamSubjects.Any(
                        examSubject =>
                            examSubject.TenantId == tenantId &&
                            classIds.Contains(
                                examSubject.ClassId)))
            .Select(exam => new StudentExamDto
            {
                ExamId = exam.ExamId,

                ExamName = exam.Name,

                StartDate = exam.StartDate,

                EndDate = exam.EndDate,

                IsActive = exam.IsActive
            })
            .OrderByDescending(x => x.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StudentResultDto>>
        GetStudentResultsAsync(
            Guid studentId,
            CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var studentExists = await _context.Students
            .AnyAsync(
                x =>
                    x.StudentId == studentId &&
                    x.TenantId == tenantId,
                cancellationToken);

        if (!studentExists)
        {
            throw new KeyNotFoundException(
                "Student not found.");
        }

        return await _context.StudentExamResults
            .AsNoTracking()
            .Where(
                x =>
                    x.StudentId == studentId &&
                    x.TenantId == tenantId &&
                    x.IsPublished)
            .Select(x => new StudentResultDto
            {
                StudentExamResultId =
                    x.StudentExamResultId,

                ExamId =
                    x.ExamId,

                ExamName =
                    x.Exam.Name,

                ExamSubjectId =
                    x.ExamSubjectId,

                SubjectId =
                    x.ExamSubject.SubjectId,

                SubjectName =
                    x.ExamSubject.Subject.SubjectName,

                ExamDate =
                    x.ExamSubject.ExamDate,

                TotalMarks =
                    x.ExamSubject.TotalMarks,

                MarksObtained =
                    x.MarksObtained,

                GradePoint =
                    x.GradePoint,

                Grade =
                    x.Grade,

                Remarks =
                    x.Remarks,

                IsPublished =
                    x.IsPublished
            })
            .OrderByDescending(x => x.ExamDate)
            .ThenBy(x => x.SubjectName)
            .ToListAsync(cancellationToken);
    }

    public async Task<StudentExamResultReportDto?>
        GetStudentExamResultsAsync(
            Guid studentId,
            Guid examId,
            CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var studentExists = await _context.Students
            .AnyAsync(
                x =>
                    x.StudentId == studentId &&
                    x.TenantId == tenantId,
                cancellationToken);

        if (!studentExists)
        {
            throw new KeyNotFoundException(
                "Student not found.");
        }

        return await _context.StudentExamResults
            .AsNoTracking()
            .Where(
                x =>
                    x.StudentId == studentId &&
                    x.TenantId == tenantId &&
                    x.ExamId == examId &&
                    x.IsPublished)
            .GroupBy(
                x => new
                {
                    x.ExamId,
                    x.Exam.Name,
                    x.Exam.StartDate,
                    x.Exam.EndDate
                })
            .Select(g => new StudentExamResultReportDto
            {
                ExamId = g.Key.ExamId,

                ExamName = g.Key.Name,

                StartDate = g.Key.StartDate,

                EndDate = g.Key.EndDate,

                Subjects = g
                    .OrderBy(
                        x =>
                            x.ExamSubject
                                .Subject
                                .SubjectName)
                    .Select(
                        x => new StudentSubjectResultDto
                        {
                            ExamSubjectId =
                                x.ExamSubjectId,

                            SubjectId =
                                x.ExamSubject.SubjectId,

                            SubjectName =
                                x.ExamSubject
                                    .Subject
                                    .SubjectName,

                            ExamDate =
                                x.ExamSubject
                                    .ExamDate,

                            TotalMarks =
                                x.ExamSubject
                                    .TotalMarks,

                            MarksObtained =
                                x.MarksObtained,

                            GradePoint =
                                x.GradePoint,

                            Grade =
                                x.Grade,

                            Remarks =
                                x.Remarks
                        })
                    .ToList()
            })
            .FirstOrDefaultAsync(
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.Students
            .AnyAsync(
                x =>
                    x.StudentId == id &&
                    x.TenantId == tenantId,
                cancellationToken);
    }

    private string GetTenantId()
    {
        var tenantId =
            _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            throw new InvalidOperationException(
                "No tenant resolved.");
        }

        return tenantId;
    }

    private static StudentDto Map(Student entity)
    {
        return new StudentDto
        {
            StudentId = entity.StudentId,

            UserId = entity.UserId,

            StudentNo = entity.StudentNo,

            FirstName = entity.FirstName,

            LastName = entity.LastName,

            DateOfBirth = entity.DateOfBirth,

            Gender = entity.Gender,

            Phone = entity.Phone,

            Email = entity.Email,

            Address = entity.Address,

            // GuardianName = entity.GuardianName,

            // GuardianPhone = entity.GuardianPhone,

            Status = entity.Status
        };
    }
}