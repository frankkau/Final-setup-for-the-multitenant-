using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CouncilRevenueCollection;

public class ApplicationDbContext
    : IdentityDbContext<User, ApplicationRole, string>
{
    private readonly ITenantService _tenantService;

    private string? CurrentTenantId => _tenantService.GetCurrentTenantId();

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantService tenantService)
        : base(options)
    {
        _tenantService = tenantService;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public override DbSet<ApplicationRole> Roles => Set<ApplicationRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureUser(builder);

        ConfigureRole(builder);

        ConfigureRefreshToken(builder);

        ConfigureTenant(builder);

        ConfigureIdentity(builder);
    }

    private void ConfigureUser(ModelBuilder builder)
    {
        builder.Entity<User>(entity =>
        {
            entity.HasQueryFilter(x =>
                string.IsNullOrWhiteSpace(CurrentTenantId) ||
                x.TenantId == CurrentTenantId);

            entity.HasIndex(x => new
            {
                x.TenantId,
                x.NormalizedUserName
            })
            .IsUnique()
            .HasDatabaseName("IX_User_Tenant_UserName");

            entity.HasIndex(x => new
            {
                x.TenantId,
                x.NormalizedEmail
            })
            .IsUnique()
            .HasDatabaseName("IX_User_Tenant_Email");

            entity.HasMany(x => x.RefreshTokens)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureRole(ModelBuilder builder)
    {
        builder.Entity<ApplicationRole>(entity =>
        {
            entity.HasQueryFilter(x =>
                string.IsNullOrWhiteSpace(CurrentTenantId) ||
                x.TenantId == CurrentTenantId);

            // Remove Identity's default global unique index
            entity.HasIndex(x => x.NormalizedName)
                .HasDatabaseName("RoleNameIndex")
                .IsUnique(false);

            entity.HasIndex(x => new
            {
                x.TenantId,
                x.NormalizedName
            })
            .IsUnique()
            .HasDatabaseName("IX_Role_Tenant_Name");
        });
    }

    private void ConfigureRefreshToken(ModelBuilder builder)
    {
        builder.Entity<RefreshToken>(entity =>
        {
            // Global tenant filter
            entity.HasQueryFilter(x =>
                string.IsNullOrWhiteSpace(CurrentTenantId) ||
                x.TenantId == CurrentTenantId);

            // Primary Key
            entity.HasKey(x => x.Id);

            // Property configuration
            entity.Property(x => x.TokenHash)
                .IsRequired()
                .HasMaxLength(64); // SHA256 hex

            entity.Property(x => x.JwtId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.CreatedByIp)
                .IsRequired()
                .HasMaxLength(45); // IPv4 / IPv6

            entity.Property(x => x.RevokedByIp)
                .HasMaxLength(45);

            entity.Property(x => x.ReplacedByTokenHash)
                .HasMaxLength(64);

            // Optimistic concurrency
            entity.Property(x => x.RowVersion)
                .IsRowVersion();

            // Indexes
            entity.HasIndex(x => new
            {
                x.TenantId,
                x.TokenHash
            })
            .IsUnique()
            .HasDatabaseName("IX_RefreshToken_Tenant_Token");

            entity.HasIndex(x => x.UserId);

            entity.HasIndex(x => x.JwtId);

            entity.HasIndex(x => x.ExpiresAt);

            entity.HasIndex(x => new
            {
                x.TenantId,
                x.RevokedAt
            });

            // Relationships
            entity.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Constraints
            entity.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_RefreshToken_Expiry",
                    "[ExpiresAt] > [CreatedAt]");
            });
        });
    }
    private void ConfigureTenant(ModelBuilder builder)
    {
        builder.Entity<Tenant>(entity =>
        {
            entity.HasIndex(x => x.Name)
                .IsUnique();

            entity.HasIndex(x => x.Subdomain)
                .IsUnique();

            entity.HasIndex(x => x.CustomDomain)
                .IsUnique()
                .HasFilter("[CustomDomain] IS NOT NULL");
        });
    }

    private void ConfigureIdentity(ModelBuilder builder)
    {
        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.HasIndex(x => x.RoleId);
        });
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyTenantTracking();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ApplyTenantTracking();

        return base.SaveChangesAsync(
            acceptAllChangesOnSuccess,
            cancellationToken);
    }

    private void ApplyTenantTracking()
    {
        var tenantId = CurrentTenantId;

        foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>())
        {
            if (entry.State == EntityState.Added)
            {
                if (!string.IsNullOrWhiteSpace(tenantId))
                {
                    entry.Property(nameof(IMustHaveTenant.TenantId)).CurrentValue = tenantId;
                }
                else if (string.IsNullOrWhiteSpace(entry.Entity.TenantId))
                {
                    throw new InvalidOperationException(
                        $"No tenant resolved while creating '{entry.Entity.GetType().Name}'.");
                }
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(IMustHaveTenant.TenantId))
                    .IsModified = false;
            }
        }
    }
}