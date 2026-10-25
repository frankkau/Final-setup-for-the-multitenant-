namespace CouncilRevenueCollection.Application.Implementation;

using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Dtos.StudentEnrollment;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;



public class StudentEnrollmentService : IStudentEnrollmentService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<StudentEnrollmentService> _logger;


    public StudentEnrollmentService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<StudentEnrollmentService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }



    public async Task<StudentEnrollmentDto> CreateAsync(
        CreateStudentEnrollmentDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();



        // Prevent duplicate enrollment
        var exists = await _context.StudentEnrollments
            .AnyAsync(
                x =>
                x.StudentId == dto.StudentId &&
                x.AcademicYearId == dto.AcademicYearId,
                cancellationToken);



        if (exists)
            throw new InvalidOperationException(
                "Student already enrolled for this academic year.");




        var enrollment = new StudentEnrollment
        {
            StudentEnrollmentId = Guid.NewGuid(),

            TenantId = tenantId,

            StudentId = dto.StudentId,

            ClassId = dto.ClassId,

            AcademicYearId = dto.AcademicYearId,

            EnrollmentDate = dto.EnrollmentDate,

            Status = dto.Status
        };



        await _context.StudentEnrollments
            .AddAsync(
                enrollment,
                cancellationToken);



        await _context.SaveChangesAsync(
            cancellationToken);



        return await GetByIdAsync(
            enrollment.StudentEnrollmentId,
            cancellationToken)
            ?? throw new Exception(
                "Enrollment creation failed.");
    }





    public async Task<StudentEnrollmentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        return await _context.StudentEnrollments
            .AsNoTracking()
            .Where(x => x.StudentEnrollmentId == id &&  x.TenantId == tenantId)
            .Select(x => new StudentEnrollmentDto
            {
                StudentEnrollmentId = x.StudentEnrollmentId,

                StudentId = x.StudentId,

                StudentName =
                    x.Student.FirstName + " " +
                    x.Student.LastName,


                ClassId = x.ClassId,

                ClassName = x.Class.ClassName,


                AcademicYearId = x.AcademicYearId,


                EnrollmentDate = x.EnrollmentDate,


                Status = x.Status

            })
            .FirstOrDefaultAsync(cancellationToken);
    }




    public async Task<IEnumerable<StudentEnrollmentDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        return await _context.StudentEnrollments
            .AsNoTracking()
            .Where(x=> x.TenantId == tenantId)
            .Select(x => new StudentEnrollmentDto
            {
                StudentEnrollmentId = x.StudentEnrollmentId,

                StudentId = x.StudentId,

                StudentName =
                    x.Student.FirstName + " " +
                    x.Student.LastName,


                ClassId = x.ClassId,

                ClassName = x.Class.ClassName,


                AcademicYearId = x.AcademicYearId,


                EnrollmentDate = x.EnrollmentDate,


                Status = x.Status

            })
            .ToListAsync(cancellationToken);
    }





    public async Task<StudentEnrollmentDto> UpdateAsync(
        Guid id,
        UpdateStudentEnrollmentDto dto,
        CancellationToken cancellationToken)
    {
       var tenantId = GetTenantId();


        var entity = await _context.StudentEnrollments
            .FirstOrDefaultAsync(
                x => x.StudentEnrollmentId == id  &&  x.TenantId == tenantId,
                cancellationToken);



        if (entity is null)
            throw new KeyNotFoundException(
                "Enrollment not found.");



        entity.ClassId = dto.ClassId;

        entity.AcademicYearId = dto.AcademicYearId;

        entity.EnrollmentDate = dto.EnrollmentDate;

        entity.Status = dto.Status;



        await _context.SaveChangesAsync(
            cancellationToken);



        return await GetByIdAsync(
            id,
            cancellationToken)
            ?? throw new Exception();
    }





    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        var entity = await _context.StudentEnrollments
            .FirstOrDefaultAsync(
                x => x.StudentEnrollmentId == id  &&  x.TenantId == tenantId,
                cancellationToken);



        if (entity is null)
            throw new KeyNotFoundException(
                "Enrollment not found.");



        _context.StudentEnrollments.Remove(entity);


        await _context.SaveChangesAsync(
            cancellationToken);
    }




    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        return await _context.StudentEnrollments
            .AnyAsync(
                x => x.StudentEnrollmentId == id  &&  x.TenantId == tenantId,
                cancellationToken);
    }




    private string GetTenantId()
    {
        var tenantId = _tenantService.GetCurrentTenantId();


        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException(
                "No tenant resolved.");


        return tenantId;
    }
}