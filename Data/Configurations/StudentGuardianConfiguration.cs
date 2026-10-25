namespace CouncilRevenueCollection.Data;

using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;




public class StudentGuardianConfiguration 
    : IEntityTypeConfiguration<StudentGuardian>
{
    private readonly string? CurrentTenantId;


    public StudentGuardianConfiguration(string? currentTenantId)
    {
        CurrentTenantId = currentTenantId;
    }



    public void Configure(EntityTypeBuilder<StudentGuardian> entity)
    {
        entity.HasKey(x => x.StudentGuardianId);



        entity.Property(x => x.TenantId)
            .IsRequired()
            .HasMaxLength(50);



        // Tenant Filter
        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId)
            || x.TenantId == CurrentTenantId);



        /*
         Student Relationship

         Student 1 ---- * StudentGuardian
        */

        entity.HasOne(x => x.Student)
            .WithMany(x => x.StudentGuardians)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);




        /*
         Guardian Relationship

         Guardian 1 ---- * StudentGuardian
        */

        entity.HasOne(x => x.Guardian)
            .WithMany(x => x.StudentGuardians)
            .HasForeignKey(x => x.GuardianId)
            .OnDelete(DeleteBehavior.Restrict);





        /*
          Tenant Relationship
        */

        entity.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);





        /*
          Prevent duplicate assignment

          Example:
          John -> Mary Guardian
          cannot be added twice
        */

        entity.HasIndex(x => new
        {
            x.TenantId,
            x.StudentId,
            x.GuardianId

        })
        .IsUnique()
        .HasDatabaseName(
            "IX_StudentGuardian_Tenant_Student_Guardian");





        /*
          Searching optimization
        */

        entity.HasIndex(x => new
        {
            x.TenantId,
            x.StudentId

        })
        .HasDatabaseName(
            "IX_StudentGuardian_Tenant_Student");



        entity.HasIndex(x => new
        {
            x.TenantId,
            x.GuardianId

        })
        .HasDatabaseName(
            "IX_StudentGuardian_Tenant_Guardian");
    }
}