using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CouncilRevenueCollection.Application.DTOs.Class;
using CouncilRevenueCollection.Application.Interfaces;
using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Entity;


namespace CouncilRevenueCollection.Infrastructure.Services;


public class ClassService : IClassService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<ClassService> _logger;


    public ClassService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<ClassService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }



    public async Task<ClassDto> CreateAsync(
        CreateClassDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        var formGradeExists = await _context.FormGrades
            .AnyAsync(
                x => x.FormGradeId == dto.FormGradeId,
                cancellationToken);


        if (!formGradeExists)
            throw new KeyNotFoundException(
                "Form grade not found.");



        var exists = await _context.Classes
            .AnyAsync(
                x => x.ClassName == dto.ClassName,
                cancellationToken);


        if (exists)
            throw new InvalidOperationException(
                "Class already exists.");



        var entity = new Class
        {
            ClassId = Guid.NewGuid(),

            TenantId = tenantId,

            FormGradeId = dto.FormGradeId,

            AcademicYearId = dto.AcademicYearId,

            ClassTeacherId = dto.ClassTeacherId,

            ClassName = dto.ClassName.Trim(),

            Capacity = dto.Capacity
        };


        await _context.Classes.AddAsync(
            entity,
            cancellationToken);


        await _context.SaveChangesAsync(
            cancellationToken);



        return await GetByIdAsync(
            entity.ClassId,
            cancellationToken)
            ?? throw new Exception("Unable to load created class.");
    }




    public async Task<ClassDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {

        var tenantId =GetTenantId();


        return await _context.Classes
            .AsNoTracking()
            .Where(x => x.ClassId == id && x.TenantId == tenantId )
            .Select(x => new ClassDto
            {
                ClassId = x.ClassId,

                TenantId = x.TenantId,

                FormGradeId = x.FormGradeId,

                FormGradeName = x.FormGrade.Name,

                AcademicYearId = x.AcademicYearId,

                ClassTeacherId = x.ClassTeacherId,

                ClassName = x.ClassName,

                Capacity = x.Capacity
            })
            .FirstOrDefaultAsync(cancellationToken);
    }




    public async Task<IEnumerable<ClassDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
       var tenantId= GetTenantId();


        return await _context.Classes
            .AsNoTracking()
            .Where(x=> x.TenantId == tenantId)
            .Select(x => new ClassDto
            {
                ClassId = x.ClassId,

                TenantId = x.TenantId,

                FormGradeId = x.FormGradeId,

                FormGradeName = x.FormGrade.Name,

                AcademicYearId = x.AcademicYearId,

                ClassTeacherId = x.ClassTeacherId,

                ClassName = x.ClassName,

                Capacity = x.Capacity
            })
            .ToListAsync(cancellationToken);
    }





    public async Task<ClassDto> UpdateAsync(
        Guid id,
        UpdateClassDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        var entity = await _context.Classes
            .FirstOrDefaultAsync(
                x => x.ClassId == id && x.TenantId == tenantId,
                cancellationToken);


        if (entity is null)
            throw new KeyNotFoundException(
                "Class not found.");



        entity.FormGradeId = dto.FormGradeId;

        entity.AcademicYearId = dto.AcademicYearId;

        entity.ClassTeacherId = dto.ClassTeacherId;

        entity.ClassName = dto.ClassName.Trim();

        entity.Capacity = dto.Capacity;



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


        var entity = await _context.Classes
            .FirstOrDefaultAsync(
                x => x.ClassId == id &&  x.TenantId == tenantId,
                cancellationToken);



        if (entity is null)
            throw new KeyNotFoundException(
                "Class not found.");



        _context.Classes.Remove(entity);


        await _context.SaveChangesAsync(
            cancellationToken);
    }




    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        return await _context.Classes
            .AnyAsync(
                x => x.ClassId == id &&  x.TenantId == tenantId,
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