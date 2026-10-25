using CouncilRevenueCollection.Application.DTOs.ClassSubject;
using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Application.Models.Dtos.ClassSubject;
using CouncilRevenueCollection.Models.Dtos.ClassSubject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagement.Domain.Entities;

namespace CouncilRevenueCollection.Application.Implementation;

public class ClassSubjectService : IClassSubjectService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<ClassSubjectService> _logger;


    public ClassSubjectService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<ClassSubjectService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }



    public async Task<IReadOnlyList<ClassSubjectListDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        return await _context.ClassSubjects
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)

            .Select(x => new ClassSubjectListDto
            {
                ClassSubjectId = x.ClassSubjectId,

                ClassName = x.Class.ClassName,

                SubjectName = x.Subject.SubjectName,

                TeacherName = x.Teacher == null
                    ? null
                    : $"{x.Teacher.FirstName} {x.Teacher.LastName}".Trim(),


                PeriodsPerWeek = x.PeriodsPerWeek,

                IsOptional = x.IsOptional,

                IsActive = x.IsActive
            })

            .OrderBy(x => x.ClassName)

            .ToListAsync(cancellationToken);
    }





    public async Task<ClassSubjectDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        return await _context.ClassSubjects

            .AsNoTracking()

            .Where(x =>
                x.ClassSubjectId == id &&
                x.TenantId == tenantId)


            .Select(x => new ClassSubjectDto
            {
                ClassSubjectId = x.ClassSubjectId,

                ClassId = x.ClassId,

                ClassName = x.Class.ClassName,


                SubjectId = x.SubjectId,

                SubjectName = x.Subject.SubjectName,


                TeacherId = x.TeacherId,


                TeacherName = x.Teacher == null
                    ? null
                    : x.Teacher.FirstName + " " + x.Teacher.LastName,


                PeriodsPerWeek = x.PeriodsPerWeek,

                IsOptional = x.IsOptional,

                IsActive = x.IsActive
            })

            .FirstOrDefaultAsync(cancellationToken);
    }






    public async Task<ClassSubjectDto> CreateAsync(
        CreateClassSubjectDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();



        var duplicate = await _context.ClassSubjects

            .AnyAsync(x =>
                x.TenantId == tenantId &&
                x.ClassId == dto.ClassId &&
                x.SubjectId == dto.SubjectId,

                cancellationToken);



        if (duplicate)

            throw new InvalidOperationException(
                "This subject is already assigned to this class.");

        var entity = new ClassSubject
        {
            ClassSubjectId = Guid.NewGuid(),

            TenantId = tenantId,

            ClassId = dto.ClassId,

            SubjectId = dto.SubjectId,

            TeacherId = dto.TeacherId,

            PeriodsPerWeek = dto.PeriodsPerWeek,

            IsOptional = dto.IsOptional,

            IsActive = dto.IsActive
        };



        await _context.ClassSubjects.AddAsync(
            entity,
            cancellationToken);



        await _context.SaveChangesAsync(
            cancellationToken);



        _logger.LogInformation(
            "Class subject {Id} created",
            entity.ClassSubjectId);



        return await GetByIdAsync(
            entity.ClassSubjectId,
            cancellationToken)

            ?? throw new InvalidOperationException(
                "Unable to load created class subject.");
    }







    public async Task<ClassSubjectDto> UpdateAsync(
        Guid id,
        UpdateClassSubjectDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();



        var entity = await _context.ClassSubjects

            .FirstOrDefaultAsync(
                x =>
                x.ClassSubjectId == id &&
                x.TenantId == tenantId,

                cancellationToken);



        if(entity is null)

            throw new KeyNotFoundException(
                "Class subject not found.");





        entity.TeacherId = dto.TeacherId;

        entity.PeriodsPerWeek = dto.PeriodsPerWeek;

        entity.IsOptional = dto.IsOptional;

        entity.IsActive = dto.IsActive;




        await _context.SaveChangesAsync(
            cancellationToken);



        _logger.LogInformation(
            "Class subject {Id} updated",
            id);



        return await GetByIdAsync(
            id,
            cancellationToken)

            ?? throw new InvalidOperationException(
                "Unable to load updated class subject.");
    }







    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();



        var entity = await _context.ClassSubjects

            .FirstOrDefaultAsync(
                x =>
                x.ClassSubjectId == id &&
                x.TenantId == tenantId,

                cancellationToken);



        if(entity is null)

            throw new KeyNotFoundException(
                "Class subject not found.");





        _context.ClassSubjects.Remove(entity);



        await _context.SaveChangesAsync(
            cancellationToken);



        _logger.LogInformation(
            "Class subject {Id} deleted",
            id);
    }








    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();



        return await _context.ClassSubjects

            .AnyAsync(
                x =>
                x.ClassSubjectId == id &&
                x.TenantId == tenantId,

                cancellationToken);
    }






    private string GetTenantId()
    {
        var tenantId =
            _tenantService.GetCurrentTenantId();



        if(string.IsNullOrWhiteSpace(tenantId))

            throw new InvalidOperationException(
                "No tenant has been resolved.");



        return tenantId;
    }
}