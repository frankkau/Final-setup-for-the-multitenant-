using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CouncilRevenueCollection.Application.Identity;

public class TenantRoleValidator : RoleValidator<ApplicationRole>
{
    private readonly ApplicationDbContext _context;

    public TenantRoleValidator(
        IdentityErrorDescriber errors,
        ApplicationDbContext context)
        : base(errors)
    {
        _context = context;
    }

    public override async Task<IdentityResult> ValidateAsync(
        RoleManager<ApplicationRole> manager,
        ApplicationRole role)
    {
        var exists = await _context.Roles
            .IgnoreQueryFilters()
            .AnyAsync(r =>
                r.Id != role.Id &&
                r.TenantId == role.TenantId &&
                r.NormalizedName == role.NormalizedName);

        if (exists)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = nameof(IdentityErrorDescriber.DuplicateRoleName),
                    Description = $"Role '{role.Name}' already exists for this tenant."
                });
        }

        return IdentityResult.Success;
    }
}