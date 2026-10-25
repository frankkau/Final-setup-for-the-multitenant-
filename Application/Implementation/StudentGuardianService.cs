namespace CouncilRevenueCollection.Application.Implementation;

using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.StudentGuardian;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
public class StudentGuardianService : IStudentGuardianService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<StudentGuardianService> _logger;


    public StudentGuardianService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<StudentGuardianService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }



    public async Task<StudentGuardianDto> CreateAsync(
        CreateStudentGuardianDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();



        var exists = await _context.StudentGuardians
            .AnyAsync(
                x =>
                x.StudentId == dto.StudentId &&
                x.GuardianId == dto.GuardianId,
                cancellationToken);



        if (exists)
            throw new InvalidOperationException(
                "Guardian already assigned to student.");



        var entity = new StudentGuardian
        {
            StudentGuardianId = Guid.NewGuid(),

            TenantId = tenantId,

            StudentId = dto.StudentId,

            GuardianId = dto.GuardianId,

            IsPrimary = dto.IsPrimary,

            CanPickup = dto.CanPickup
        };


        await _context.StudentGuardians
            .AddAsync(entity, cancellationToken);



        await _context.SaveChangesAsync(
            cancellationToken);



        return await GetByIdAsync(
            entity.StudentGuardianId,
            cancellationToken)
            ?? throw new Exception();
    }





    public async Task<StudentGuardianDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        return await _context.StudentGuardians
            .AsNoTracking()
            .Where(x => x.StudentGuardianId == id  &&  x.TenantId == tenantId)
            .Select(x => new StudentGuardianDto
            {
                StudentGuardianId = x.StudentGuardianId,


                StudentId = x.StudentId,

                StudentName =
                    x.Student.FirstName + " " +
                    x.Student.LastName,


                GuardianId = x.GuardianId,

                GuardianName =
                    x.Guardian.FirstName + " " +
                    x.Guardian.LastName,


                IsPrimary = x.IsPrimary,

                CanPickup = x.CanPickup

            })
            .FirstOrDefaultAsync(cancellationToken);
    }





    public async Task<IEnumerable<StudentGuardianDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        return await _context.StudentGuardians
            .AsNoTracking()
            .Where(x=>x.TenantId == tenantId)
            .Select(x => new StudentGuardianDto
            {
                StudentGuardianId = x.StudentGuardianId,


                StudentId = x.StudentId,

                StudentName =
                    x.Student.FirstName + " " +
                    x.Student.LastName,


                GuardianId = x.GuardianId,

                GuardianName =
                    x.Guardian.FirstName + " " +
                    x.Guardian.LastName,


                IsPrimary = x.IsPrimary,

                CanPickup = x.CanPickup

            })
            .ToListAsync(cancellationToken);
    }





    public async Task<StudentGuardianDto> UpdateAsync(
        Guid id,
        UpdateStudentGuardianDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        var entity = await _context.StudentGuardians
            .FirstOrDefaultAsync(
                x => x.StudentGuardianId == id  &&  x.TenantId == tenantId,
                cancellationToken);



        if(entity is null)
            throw new KeyNotFoundException(
                "Relationship not found.");



        entity.IsPrimary = dto.IsPrimary;

        entity.CanPickup = dto.CanPickup;



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


        var entity = await _context.StudentGuardians
            .FirstOrDefaultAsync(
                x => x.StudentGuardianId == id &&  x.TenantId == tenantId,
                cancellationToken);



        if(entity is null)
            throw new KeyNotFoundException();



        _context.StudentGuardians.Remove(entity);


        await _context.SaveChangesAsync(
            cancellationToken);
    }





    public async Task<IEnumerable<StudentGuardianDto>> GetStudentGuardiansAsync(
        Guid studentId,
        CancellationToken cancellationToken)
    {
       var tenantId = GetTenantId();


        return await _context.StudentGuardians
            .Where(x => x.StudentId == studentId  &&  x.TenantId == tenantId)
            .Select(x => new StudentGuardianDto
            {
                StudentGuardianId = x.StudentGuardianId,

                GuardianId = x.GuardianId,

                GuardianName =
                    x.Guardian.FirstName + " " +
                    x.Guardian.LastName,


                IsPrimary = x.IsPrimary,

                CanPickup = x.CanPickup

            })
            .ToListAsync(cancellationToken);
    }





    public async Task<IEnumerable<StudentGuardianDto>> GetGuardianStudentsAsync(
        Guid guardianId,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        return await _context.StudentGuardians
            .Where(x => x.GuardianId == guardianId  &&  x.TenantId == tenantId)
            .Select(x => new StudentGuardianDto
            {
                StudentGuardianId = x.StudentGuardianId,


                StudentId = x.StudentId,

                StudentName =
                    x.Student.FirstName + " " +
                    x.Student.LastName,


                IsPrimary = x.IsPrimary,

                CanPickup = x.CanPickup

            })
            .ToListAsync(cancellationToken);
    }




    private string GetTenantId()
    {
        var tenantId = _tenantService.GetCurrentTenantId();


        if(string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException(
                "Tenant not resolved.");


        return tenantId;
    }
}