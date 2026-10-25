using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Domain.Entities;
using CouncilRevenueCollection.Models;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;

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
    public DbSet<AcademicStructure> AcademicStructures => Set<AcademicStructure>();
    public DbSet<FormGrade> FormGrades => Set<FormGrade>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<StudentEnrollment> StudentEnrollments => Set<StudentEnrollment>();
    public DbSet<Guardian> Guardians => Set<Guardian>();
    public DbSet<StudentGuardian> StudentGuardians => Set<StudentGuardian>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<ClassSubject> ClassSubjects => Set<ClassSubject>();
    public DbSet<TeacherSubject> TeacherSubjects => Set<TeacherSubject>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<ExamSubject> ExamSubjects => Set<ExamSubject>();   
    public DbSet<StudentExamResult> StudentExamResults => Set<StudentExamResult>();
    public DbSet<StudentAttendance> StudentAttendances => Set<StudentAttendance>();
    public DbSet<Timetable> Timetables => Set<Timetable>();
    public DbSet<FeeStructure> FeeStructures => Set<FeeStructure>();
    public DbSet<FeePayment> FeePayments => Set<FeePayment>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
    public DbSet<SchoolExpense> SchoolExpenses => Set<SchoolExpense>();
    public DbSet<ExpenseReceipt> ExpenseReceipts => Set<ExpenseReceipt>();
    public DbSet<LibraryBook> LibraryBooks => Set<LibraryBook>();
    public DbSet<LibraryBookCopy> LibraryBookCopies => Set<LibraryBookCopy>();
    public DbSet<LibraryLoan> LibraryLoans => Set<LibraryLoan>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Registrar> Registrars => Set<Registrar>();
   public DbSet<Accountant> Accountants => Set<Accountant>();

   protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureUser(builder);
        ConfigureRole(builder);
        ConfigureRefreshToken(builder);
        ConfigureTenant(builder);
        ConfigureIdentity(builder);

        ConfigureAcademicStructure(builder);
        ConfigureFormGrade(builder);
        ConfigureTeacher(builder);
        ConfigureClass(builder);
        ConfigureStudent(builder);

        ConfigureGuardian(builder);

        ConfigureStudentGuardian(builder);
        ConfigureStudentEnrollment(builder);
        ConfigureSubject(builder);
        ConfigureClassSubject(builder);
        ConfigureTeacherSubject(builder);
        ConfigureExam(builder);
        ConfigureExamSubject(builder);
        ConfigureStudentExamResult(builder);
        ConfigureStudentAttendance(builder);
        ConfigureTimetable(builder);
        ConfigureFeeStructure(builder);
        ConfigureFeePayment(builder);
        ConfigureInvoice(builder);
        ConfigureExpenseCategory(builder);
        ConfigureSchoolExpense(builder);
        ConfigureExpenseReceipt(builder);
        ConfigureLibraryBook(builder);
        ConfigureLibraryBookCopy(builder);
        ConfigureLibraryLoan(builder);
        ConfigureNotification(builder);
        ConfigureDepartment(builder);
        ConfigureRegistrar(builder);
        ConfigureAccountant(builder);
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
private void ConfigureSubject(ModelBuilder builder)
{
    builder.Entity<Subject>(entity =>
    {
        entity.HasKey(x => x.SubjectId);

        entity.Property(x => x.SubjectCode)
            .IsRequired()
            .HasMaxLength(20);

        entity.Property(x => x.SubjectName)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(x => x.Description)
            .HasMaxLength(500);

        entity.Property(x => x.IsCompulsory)
            .IsRequired();

        entity.Property(x => x.IsActive)
            .IsRequired();

        // Tenant Filter
        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId) ||
            x.TenantId == CurrentTenantId);

        // Tenant Relationship
        entity.HasOne(x => x.Tenant)
            .WithMany(x => x.Subjects)
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);

        // Subject Code unique per tenant
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.SubjectCode
        })
        .IsUnique()
        .HasDatabaseName("IX_Subject_Tenant_Code");

        // Subject Name unique per tenant
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.SubjectName
        })
        .IsUnique()
        .HasDatabaseName("IX_Subject_Tenant_Name");
    });
}
private void ConfigureStudentExamResult(ModelBuilder builder)
{
    builder.Entity<StudentExamResult>(entity =>
    {
        entity.HasKey(x => x.StudentExamResultId);


        // -----------------------------
        // Properties
        // -----------------------------

        entity.Property(x => x.MarksObtained)
            .HasPrecision(6, 2)
            .IsRequired();

        entity.Property(x => x.GradePoint)
            .HasPrecision(5, 2);

        entity.Property(x => x.Grade)
            .HasMaxLength(20);

        entity.Property(x => x.Remarks)
            .HasMaxLength(500);

        entity.Property(x => x.IsPublished)
            .IsRequired();


        // -----------------------------
        // Tenant isolation
        // -----------------------------

        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId) ||
            x.TenantId == CurrentTenantId);


        // -----------------------------
        // Tenant relationship
        // -----------------------------

        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        // -----------------------------
        // Student relationship
        // -----------------------------

        entity.HasOne(x => x.Student)
            .WithMany(x => x.ExamResults)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);


        // -----------------------------
        // Exam relationship
        // -----------------------------

        entity.HasOne(x => x.Exam)
            .WithMany(x => x.StudentExamResults)
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Restrict);


        // -----------------------------
        // ExamSubject relationship
        // -----------------------------

        entity.HasOne(x => x.ExamSubject)
            .WithMany(x => x.StudentExamResults)
            .HasForeignKey(x => x.ExamSubjectId)
            .OnDelete(DeleteBehavior.Restrict);


        // -----------------------------
        // Prevent duplicate results
        // -----------------------------

        entity.HasIndex(x => new
        {
            x.TenantId,
            x.StudentId,
            x.ExamId,
            x.ExamSubjectId
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_StudentExamResult_Tenant_Student_Exam_Subject");
    });
}
private void ConfigureTimetable(ModelBuilder builder)
{
    builder.Entity<Timetable>(entity =>
    {
        entity.HasKey(x => x.TimetableId);


        // Tenant isolation
        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId)
            ||
            x.TenantId == CurrentTenantId);


        // Tenant relationship
        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        // Timetable -> ClassSubject
        entity.HasOne(x => x.ClassSubject)
            .WithMany(x => x.Timetables)
            .HasForeignKey(x => x.ClassSubjectId)
            .OnDelete(DeleteBehavior.Restrict);


        // Room number
        entity.Property(x => x.RoomNumber)
            .HasMaxLength(50);


        // Time configuration
        entity.Property(x => x.StartTime)
            .HasColumnType("time");

        entity.Property(x => x.EndTime)
            .HasColumnType("time");


        // Prevent exact duplicate timetable entries
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.ClassSubjectId,
            x.DayOfWeek,
            x.StartTime,
            x.EndTime
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_Timetable_Tenant_ClassSubject_Day_Time");
    });
}
private void ConfigureExpenseReceipt(ModelBuilder builder)
{
    builder.Entity<ExpenseReceipt>(entity =>
    {
        entity.HasKey(x => x.ExpenseReceiptId);


        entity.Property(x => x.TenantId)
            .IsRequired();


        entity.Property(x => x.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);


        entity.Property(x => x.StoredFileName)
            .IsRequired()
            .HasMaxLength(255);


        entity.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(100);


        entity.Property(x => x.FileSize)
            .IsRequired();


        entity.Property(x => x.FileHash)
            .IsRequired()
            .HasMaxLength(64);


        entity.Property(x => x.ReceiptDate)
            .IsRequired();


        entity.Property(x => x.ReceiptNumber)
            .HasMaxLength(100);


        entity.Property(x => x.ReceiptAmount)
            .HasPrecision(18, 2);


        entity.Property(x => x.StoragePath)
            .HasMaxLength(500);


        entity.Property(x => x.UploadedAtUtc)
            .IsRequired();


        // Tenant isolation
        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId)
            ||
            x.TenantId == CurrentTenantId);


        // Tenant relationship
        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        // School expense relationship
        entity.HasOne(x => x.SchoolExpense)
            .WithMany(x => x.Receipts)
            .HasForeignKey(x => x.SchoolExpenseId)
            .OnDelete(DeleteBehavior.Restrict);


        // Prevent the same physical receipt
        // from being uploaded more than once
        // within the same tenant.
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.FileHash
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_ExpenseReceipt_Tenant_FileHash");
    });
}
private void ConfigureStudentAttendance(ModelBuilder builder)
{
    builder.Entity<StudentAttendance>(entity =>
    {
        entity.HasKey(x => x.StudentAttendanceId);


        entity.Property(x => x.Status)
            .IsRequired();


        entity.Property(x => x.Remarks)
            .HasMaxLength(500);


        // Tenant isolation
        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId)
            ||
            x.TenantId == CurrentTenantId);


        // Tenant relationship
        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        // Enrollment relationship
       entity.HasOne(x => x.StudentEnrollment)
            .WithMany(x => x.Attendances)
            .HasForeignKey(x => x.StudentEnrollmentId)
            .OnDelete(DeleteBehavior.Restrict);


        // Prevent duplicate attendance
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.StudentEnrollmentId,
            x.AttendanceDate
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_Attendance_Enrollment_Date");
    });
}
private void ConfigureAccountant(ModelBuilder builder)
{
    builder.Entity<Accountant>(entity =>
    {
        entity.HasKey(x => x.AccountantId);

        entity.Property(x => x.EmployeeNo)
            .IsRequired()
            .HasMaxLength(30);

        entity.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(150);

        entity.Property(x => x.Phone)
            .HasMaxLength(20);

        entity.Property(x => x.Status)
            .IsRequired();

        entity.Property(x => x.CreatedAt)
            .IsRequired();

        // Tenant isolation
        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId) ||
            x.TenantId == CurrentTenantId);

        // Accountant -> Tenant
        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);

        // Accountant -> User
        entity.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Accountant>(x => x.UserId)
            .HasPrincipalKey<User>(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);

        // Accountant -> Department
        entity.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Employee number unique per tenant
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.EmployeeNo
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_Accountant_Tenant_EmployeeNo");

        // One User -> one Accountant per tenant
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.UserId
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_Accountant_Tenant_User");

        // Department lookup
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.DepartmentId
        })
        .HasDatabaseName(
            "IX_Accountant_Tenant_Department");
    });
}
private void ConfigureLibraryBook(ModelBuilder builder)
{
    builder.Entity<LibraryBook>(entity =>
    {
        entity.HasKey(x => x.LibraryBookId);


        entity.Property(x => x.ISBN)
            .IsRequired()
            .HasMaxLength(30);


        entity.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);


        entity.Property(x => x.Author)
            .IsRequired()
            .HasMaxLength(200);


        entity.Property(x => x.Publisher)
            .HasMaxLength(200);


        entity.Property(x => x.Edition)
            .HasMaxLength(100);


        entity.Property(x => x.Category)
            .HasMaxLength(100);


        entity.Property(x => x.Description)
            .HasMaxLength(1000);


        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId)
            ||
            x.TenantId == CurrentTenantId);


        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        entity.HasIndex(x => new
        {
            x.TenantId,
            x.ISBN
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_LibraryBook_Tenant_ISBN");
    });
}
private void ConfigureNotification(ModelBuilder builder)
{
    builder.Entity<Notification>(entity =>
    {
        entity.HasKey(x => x.NotificationId);


        entity.Property(x => x.TenantId)
            .IsRequired();


        entity.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(450);


        entity.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);


        entity.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(1000);


        entity.Property(x => x.NotificationType)
            .HasMaxLength(100);


        entity.Property(x => x.CreatedAtUtc)
            .IsRequired();


        entity.Property(x => x.IsRead)
            .IsRequired();


        // Tenant isolation
        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId)
            ||
            x.TenantId == CurrentTenantId);


        // Tenant relationship
        entity.HasOne(x => x.Tenant)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        // User relationship
        entity.HasOne(x => x.User)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.UserId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        // Optimizes:
        // GetMyNotificationsAsync()
        // GetUnreadAsync()
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.UserId,
            x.CreatedAtUtc
        })
        .HasDatabaseName(
            "IX_Notification_Tenant_User_Created");


        // Optimizes unread notification queries.
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.UserId,
            x.IsRead
        })
        .HasDatabaseName(
            "IX_Notification_Tenant_User_Read");
    });
}
private void ConfigureLibraryBookCopy(ModelBuilder builder)
{
    builder.Entity<LibraryBookCopy>(entity =>
    {
        entity.HasKey(x => x.LibraryBookCopyId);


        entity.Property(x => x.AccessionNumber)
            .IsRequired()
            .HasMaxLength(50);


        entity.Property(x => x.ShelfLocation)
            .HasMaxLength(100);


        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId)
            ||
            x.TenantId == CurrentTenantId);


        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        entity.HasOne(x => x.LibraryBook)
            .WithMany(x => x.Copies)
            .HasForeignKey(x => x.LibraryBookId)
            .OnDelete(DeleteBehavior.Restrict);


        entity.HasIndex(x => new
        {
            x.TenantId,
            x.AccessionNumber
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_LibraryBookCopy_Tenant_Accession");
    });
}
private void ConfigureLibraryLoan(ModelBuilder builder)
{
    builder.Entity<LibraryLoan>(entity =>
    {
        entity.HasKey(x => x.LibraryLoanId);


        entity.Property(x => x.FineAmount)
            .HasPrecision(18, 2);


        entity.Property(x => x.Remarks)
            .HasMaxLength(500);


        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId)
            ||
            x.TenantId == CurrentTenantId);


        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        entity.HasOne(x => x.LibraryBookCopy)
            .WithMany(x => x.Loans)
            .HasForeignKey(x => x.LibraryBookCopyId)
            .OnDelete(DeleteBehavior.Restrict);


        entity.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);


        entity.HasIndex(x => new
        {
            x.TenantId,
            x.StudentId
        })
        .HasDatabaseName(
            "IX_LibraryLoan_Tenant_Student");


        entity.HasIndex(x => new
        {
            x.TenantId,
            x.LibraryBookCopyId
        })
        .HasDatabaseName(
            "IX_LibraryLoan_Tenant_Copy");
    });
}
private void ConfigureFeeStructure(ModelBuilder builder)
{
    builder.Entity<FeeStructure>(entity =>
    {
        entity.HasKey(x => x.FeeStructureId);


        entity.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);


        entity.Property(x => x.Amount)
            .HasPrecision(18, 2);


        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId)
            ||
            x.TenantId == CurrentTenantId);


        // Tenant relationship
        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        // Class relationship
        entity.HasOne(x => x.Class)
            .WithMany()
            .HasForeignKey(x => x.ClassId)
            .OnDelete(DeleteBehavior.Restrict);


        // Prevent duplicate fee names
        // for the same class within a tenant
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.ClassId,
            x.Name
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_FeeStructure_Tenant_Class_Name");
    });
}
private void ConfigureInvoice(ModelBuilder builder)
{
    builder.Entity<Invoice>(entity =>
    {
        entity.HasKey(x => x.InvoiceId);


        entity.Property(x => x.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);


        entity.Property(x => x.Amount)
            .HasPrecision(18, 2);


        entity.Property(x => x.AmountPaid)
            .HasPrecision(18, 2);


        entity.Property(x => x.Balance)
            .HasPrecision(18, 2);


        entity.Property(x => x.Remarks)
            .HasMaxLength(500);


        // Tenant isolation
        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId)
            ||
            x.TenantId == CurrentTenantId);


        // Tenant relationship
        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);


        // Student relationship
        entity.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);


        // Fee structure relationship
        entity.HasOne(x => x.FeeStructure)
            .WithMany()
            .HasForeignKey(x => x.FeeStructureId)
            .OnDelete(DeleteBehavior.Restrict);


        // Invoice number unique per tenant
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.InvoiceNumber
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_Invoice_Tenant_InvoiceNumber");


        // Student invoice lookup
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.StudentId
        })
        .HasDatabaseName(
            "IX_Invoice_Tenant_Student");


        // Fee structure lookup
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.FeeStructureId
        })
        .HasDatabaseName(
            "IX_Invoice_Tenant_FeeStructure");
    });
}
private void ConfigureExpenseCategory(ModelBuilder builder)
{
    builder.Entity<ExpenseCategory>(entity =>
    {
        entity.HasKey(x => x.ExpenseCategoryId);


        entity.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);


        entity.Property(x => x.Description)
            .HasMaxLength(500);


        entity.Property(x => x.IsActive)
            .IsRequired();


        // Tenant isolation
        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId)
            ||
            x.TenantId == CurrentTenantId);


        // Tenant relationship
        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        // Category name must be unique per tenant
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.Name
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_ExpenseCategory_Tenant_Name");
    });
}
private void ConfigureSchoolExpense(ModelBuilder builder)
{
    builder.Entity<SchoolExpense>(entity =>
    {
        entity.HasKey(x => x.SchoolExpenseId);


        entity.Property(x => x.ExpenseNumber)
            .IsRequired()
            .HasMaxLength(50);


        entity.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);


        entity.Property(x => x.Amount)
            .HasPrecision(18, 2)
            .IsRequired();


        entity.Property(x => x.PaymentMethod)
            .IsRequired()
            .HasMaxLength(50);


        entity.Property(x => x.ReferenceNumber)
            .HasMaxLength(100);


        entity.Property(x => x.Payee)
            .HasMaxLength(200);


        entity.Property(x => x.Remarks)
            .HasMaxLength(1000);


        entity.Property(x => x.IsApproved)
            .IsRequired();


        entity.Property(x => x.IsCancelled)
            .IsRequired();


        // Tenant isolation
        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId)
            ||
            x.TenantId == CurrentTenantId);


        // Tenant relationship
        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        // Expense category
        entity.HasOne(x => x.ExpenseCategory)
            .WithMany(x => x.SchoolExpenses)
            .HasForeignKey(x => x.ExpenseCategoryId)
            .OnDelete(DeleteBehavior.Restrict);


        // Expense number unique per tenant
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.ExpenseNumber
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_SchoolExpense_Tenant_ExpenseNumber");


        // Useful for financial reporting
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.AcademicYearId,
            x.TermId
        })
        .HasDatabaseName(
            "IX_SchoolExpense_Tenant_Year_Term");


        // Useful for date-based reports
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.ExpenseDate
        })
        .HasDatabaseName(
            "IX_SchoolExpense_Tenant_ExpenseDate");
    });
}
private void ConfigureFeePayment(ModelBuilder builder)
{
    builder.Entity<FeePayment>(entity =>
    {
        entity.HasKey(x => x.FeePaymentId);


        entity.Property(x => x.AmountPaid)
            .HasPrecision(18, 2);


        entity.Property(x => x.PaymentMethod)
            .IsRequired()
            .HasMaxLength(50);


        entity.Property(x => x.ReferenceNumber)
            .HasMaxLength(100);


        entity.Property(x => x.Remarks)
            .HasMaxLength(500);


        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId)
            ||
            x.TenantId == CurrentTenantId);


        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        entity.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);


        entity.HasOne(x => x.FeeStructure)
            .WithMany(x => x.FeePayments)
            .HasForeignKey(x => x.FeeStructureId)
            .OnDelete(DeleteBehavior.Restrict);


        entity.HasIndex(x => new
        {
            x.TenantId,
            x.ReferenceNumber
        })
        .IsUnique()
        .HasFilter("\"ReferenceNumber\" IS NOT NULL")
        .HasDatabaseName(
            "IX_FeePayment_Tenant_Reference");
    });
}
private void ConfigureDepartment(ModelBuilder builder)
{
    builder.Entity<Department>(entity =>
    {
        entity.HasKey(x => x.DepartmentId);


        entity.Property(x => x.TenantId)
            .IsRequired();


        entity.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);


        entity.Property(x => x.Description)
            .HasMaxLength(500);


        // Tenant isolation
        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId)
            ||
            x.TenantId == CurrentTenantId);


        // Tenant relationship
        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        // Department → Teachers
        entity.HasMany(x => x.Teachers)
            .WithOne(x => x.Department)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);


        // Department name must be unique per tenant
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.Name
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_Department_Tenant_Name");
    });
}
private void ConfigureAcademicStructure(ModelBuilder builder)
    {
        builder.Entity<AcademicStructure>(entity =>
        {
            entity.HasKey(x => x.AcademicStructureId);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.Property(x => x.Sequence)
                .IsRequired();

            // Global Query Filter
            entity.HasQueryFilter(x =>
                string.IsNullOrWhiteSpace(CurrentTenantId) ||
                x.TenantId == CurrentTenantId);

            // Relationship
            entity.HasOne(x => x.Tenant)
                .WithMany(x => x.AcademicStructures)
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // A tenant cannot have duplicate academic structure names
            entity.HasIndex(x => new
            {
                x.TenantId,
                x.Name
            })
            .IsUnique()
            .HasDatabaseName("IX_AcademicStructure_Tenant_Name");

            // Optional: Prevent duplicate sequence numbers within a tenant
            entity.HasIndex(x => new
            {
                x.TenantId,
                x.Sequence
            })
            .IsUnique()
            .HasDatabaseName("IX_AcademicStructure_Tenant_Sequence");
        });
    }
private void ConfigureClassSubject(ModelBuilder builder)
{
    builder.Entity<ClassSubject>(entity =>
    {
        entity.HasKey(x => x.ClassSubjectId);


        entity.Property(x => x.PeriodsPerWeek)
            .IsRequired();


        entity.Property(x => x.IsOptional)
            .IsRequired();


        entity.Property(x => x.IsActive)
            .IsRequired();



        // Tenant filter
        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId) ||
            x.TenantId == CurrentTenantId);



        // Tenant relationship
        entity.HasOne(x => x.Tenant)
            .WithMany(x => x.ClassSubjects)
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);



        // Class relationship
        entity.HasOne(x => x.Class)
            .WithMany(x => x.ClassSubjects)
            .HasForeignKey(x => x.ClassId)
            .OnDelete(DeleteBehavior.Restrict);



        // Subject relationship
        entity.HasOne(x => x.Subject)
            .WithMany(x => x.ClassSubjects)
            .HasForeignKey(x => x.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);



        // Optional Teacher relationship
        entity.HasOne(x => x.Teacher)
            .WithMany(x => x.ClassSubjects)
            .HasForeignKey(x => x.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);



        // Prevent duplicate subject assignment
        // Same subject cannot be added twice to same class
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.ClassId,
            x.SubjectId
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_ClassSubject_Tenant_Class_Subject");



        // Useful indexes for searching
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.ClassId
        })
        .HasDatabaseName(
            "IX_ClassSubject_Tenant_Class");



        entity.HasIndex(x => new
        {
            x.TenantId,
            x.SubjectId
        })
        .HasDatabaseName(
            "IX_ClassSubject_Tenant_Subject");

    });
}
private void ConfigureExamSubject(ModelBuilder builder)
{
    builder.Entity<ExamSubject>(entity =>
    {
        entity.HasKey(x => x.ExamSubjectId);


        entity.Property(x => x.TotalMarks)
            .HasPrecision(6, 2)
            .IsRequired();


        entity.Property(x => x.ExamDate)
            .IsRequired();


        entity.Property(x => x.StartTime)
            .IsRequired();


        entity.Property(x => x.EndTime)
            .IsRequired();


        entity.Property(x => x.IsActive)
            .IsRequired();


        // Tenant isolation
        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId) ||
            x.TenantId == CurrentTenantId);


        // Tenant relationship
        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        // Exam relationship
        entity.HasOne(x => x.Exam)
            .WithMany(x => x.ExamSubjects)
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Restrict);


        // Class relationship
        entity.HasOne(x => x.Class)
            .WithMany()
            .HasForeignKey(x => x.ClassId)
            .OnDelete(DeleteBehavior.Restrict);


        // Subject relationship
        entity.HasOne(x => x.Subject)
            .WithMany()
            .HasForeignKey(x => x.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);


        // Optional teacher relationship
        entity.HasOne(x => x.Teacher)
            .WithMany()
            .HasForeignKey(x => x.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);


        // A subject can only be assigned once
        // to a class within an exam.
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.ExamId,
            x.ClassId,
            x.SubjectId
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_ExamSubject_Tenant_Exam_Class_Subject");
    });
}
private void ConfigureExam(ModelBuilder builder)
{
    builder.Entity<Exam>(entity =>
    {
        entity.HasKey(x => x.ExamId);


        entity.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);


        entity.Property(x => x.StartDate)
            .IsRequired();


        entity.Property(x => x.EndDate)
            .IsRequired();


        entity.Property(x => x.IsActive)
            .IsRequired();


        // Tenant isolation
        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId) ||
            x.TenantId == CurrentTenantId);


        // Tenant relationship
        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        // Exam → ExamSubject
        entity.HasMany(x => x.ExamSubjects)
            .WithOne(x => x.Exam)
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Restrict);


        // Unique exam name per tenant
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.Name
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_Exam_Tenant_Name");
    });
}
private void ConfigureTeacherSubject(ModelBuilder builder)
{
    builder.Entity<TeacherSubject>(entity =>
    {
        entity.HasKey(x => x.TeacherSubjectId);

        // Tenant isolation
        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId) ||
            x.TenantId == CurrentTenantId);

        // Tenant relationship
        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);

        // Teacher relationship
        entity.HasOne(x => x.Teacher)
            .WithMany(x => x.TeacherSubjects)
            .HasForeignKey(x => x.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Subject relationship
        entity.HasOne(x => x.Subject)
            .WithMany(x => x.TeacherSubjects)
            .HasForeignKey(x => x.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // A teacher can only be assigned to the same subject once
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.TeacherId,
            x.SubjectId
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_TeacherSubject_Tenant_Teacher_Subject");
    });
}
private void ConfigureStudentEnrollment(ModelBuilder builder)
{
    builder.Entity<StudentEnrollment>(entity =>
    {
        entity.HasKey(x => x.StudentEnrollmentId);



        entity.HasOne(x => x.Student)
            .WithMany(x => x.Enrollments)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);



        entity.HasOne(x => x.Class)
            .WithMany(x => x.StudentEnrollments)
            .HasForeignKey(x => x.ClassId)
            .OnDelete(DeleteBehavior.Restrict);



        entity.HasOne(x => x.Tenant)
            .WithMany(y => y.StudentEnrollments)
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);



        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId)
            ||
            x.TenantId == CurrentTenantId);



        // A student cannot have two classes
        // in the same academic year

        entity.HasIndex(x => new
        {
            x.StudentId,
            x.AcademicYearId
        })
        .IsUnique();

    });
}
private void ConfigureTeacher(ModelBuilder builder)
{
    builder.Entity<Teacher>(entity =>
    {
        entity.HasKey(x => x.TeacherId);


        // =========================
        // Properties
        // =========================

        entity.Property(x => x.TenantId)
            .IsRequired()
            .HasMaxLength(100);


        entity.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(450);


        entity.Property(x => x.EmployeeNo)
            .IsRequired()
            .HasMaxLength(30);


        entity.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);


        entity.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);


        entity.Property(x => x.Email)
            .HasMaxLength(150);


        entity.Property(x => x.Phone)
            .HasMaxLength(20);


        entity.Property(x => x.Qualification)
            .HasMaxLength(200);


        entity.Property(x => x.HireDate)
            .IsRequired();


        entity.Property(x => x.Status)
            .IsRequired();


        // =========================
        // Tenant isolation
        // =========================

        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId) ||
            x.TenantId == CurrentTenantId);


        // =========================
        // Teacher → Tenant
        // =========================

        entity.HasOne(x => x.Tenant)
            .WithMany(x => x.Teachers)
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        // =========================
        // Teacher → User
        // Required one-to-one
        // =========================

        entity.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Teacher>(x => x.UserId)
            .HasPrincipalKey<User>(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        // =========================
        // Teacher → Department
        // Optional
        // =========================

        entity.HasOne(x => x.Department)
            .WithMany(x => x.Teachers)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);


        // =========================
        // Indexes
        // =========================

        // Employee number unique per tenant
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.EmployeeNo
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_Teacher_Tenant_EmployeeNo");


        // One Teacher per User
        entity.HasIndex(x => x.UserId)
            .IsUnique()
            .HasDatabaseName(
                "IX_Teacher_UserId");


        // Department lookup
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.DepartmentId
        })
        .HasDatabaseName(
            "IX_Teacher_Tenant_Department");
    });
}
private void ConfigureFormGrade(ModelBuilder builder)
        {
            builder.Entity<FormGrade>(entity =>
            {
                entity.HasKey(x => x.FormGradeId);



                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);



                entity.Property(x => x.Sequence)
                    .IsRequired();



                entity.HasQueryFilter(x =>
                    string.IsNullOrWhiteSpace(CurrentTenantId) ||
                    x.TenantId == CurrentTenantId);



                // Tenant relationship (no navigation property on FormGrade)
                entity.HasOne<Tenant>()
                    .WithMany()
                    .HasForeignKey(x => x.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);




                // AcademicStructure -> FormGrades
                entity.HasOne(x => x.AcademicStructure)
                    .WithMany(x => x.FormGrades)
                    .HasForeignKey(x => x.AcademicStructureId)
                    .OnDelete(DeleteBehavior.Restrict);



                // Grade name unique per academic level
                entity.HasIndex(x => new
                {
                    x.TenantId,
                    x.AcademicStructureId,
                    x.Name
                })
                .IsUnique()
                .HasDatabaseName("IX_FormGrade_Tenant_Structure_Name");



                // Sequence unique per academic level
                entity.HasIndex(x => new
                {
                    x.TenantId,
                    x.AcademicStructureId,
                    x.Sequence
                })
                .IsUnique()
                .HasDatabaseName("IX_FormGrade_Tenant_Structure_Sequence");

            });
                }
private void ConfigureClass(ModelBuilder builder)
{
    builder.Entity<Class>(entity =>
    {
        entity.HasKey(x => x.ClassId);

        entity.Property(x => x.ClassName)
            .IsRequired()
            .HasMaxLength(100);

        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId) ||
            x.TenantId == CurrentTenantId);

        // Tenant relationship
       entity.HasOne(x => x.Tenant)
            .WithMany(t => t.Classes)
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict); 

        // FormGrade relationship
        entity.HasOne(x => x.FormGrade)
            .WithMany(x => x.Classes)
            .HasForeignKey(x => x.FormGradeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Teacher relationship
        entity.HasOne(x => x.ClassTeacher)
            .WithMany(x => x.Classes)
            .HasForeignKey(x => x.ClassTeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(x => new
        {
            x.TenantId,
            x.AcademicYearId,
            x.ClassName
        })
        .IsUnique()
        .HasDatabaseName("IX_Class_Tenant_Year_Name");
    });
}
private void ConfigureGuardian(ModelBuilder builder)
{
    builder.Entity<Guardian>(entity =>
    {
        entity.HasKey(x => x.GuardianId);

        entity.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(x => x.Relationship)
            .IsRequired()
            .HasMaxLength(50);

        entity.Property(x => x.Phone)
            .IsRequired()
            .HasMaxLength(20);

        entity.Property(x => x.Email)
            .HasMaxLength(150);

        entity.Property(x => x.Address)
            .HasMaxLength(300);

        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId) ||
            x.TenantId == CurrentTenantId);

        // Tenant relationship
        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Guardian -> StudentGuardian
        entity.HasMany(x => x.StudentGuardians)
            .WithOne(x => x.Guardian)
            .HasForeignKey(x => x.GuardianId)
            .OnDelete(DeleteBehavior.Restrict);

        // Email unique within a tenant (optional because email may be blank)
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.Email
        })
        .IsUnique()
        .HasDatabaseName("IX_Guardian_Tenant_Email")
        .HasFilter("[Email] IS NOT NULL");

        // Phone should be unique per tenant
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.Phone
        })
        .IsUnique()
        .HasDatabaseName("IX_Guardian_Tenant_Phone");

        // Fast searching by name
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.LastName,
            x.FirstName
        })
        .HasDatabaseName("IX_Guardian_Tenant_Name");
    });
}
private void ConfigureStudent(ModelBuilder builder)
{
    builder.Entity<Student>(entity =>
    {
        entity.HasKey(x => x.StudentId);


        // =========================
        // Properties
        // =========================

        entity.Property(x => x.TenantId)
            .IsRequired()
            .HasMaxLength(100);


        entity.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(450);


        entity.Property(x => x.StudentNo)
            .IsRequired()
            .HasMaxLength(30);


        entity.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);


        entity.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);


        entity.Property(x => x.Gender)
            .HasMaxLength(20);


        entity.Property(x => x.Phone)
            .HasMaxLength(20);


        entity.Property(x => x.Email)
            .HasMaxLength(150);



        entity.Property(x => x.Address)
            .HasMaxLength(300);


        // =========================
        // Tenant isolation
        // =========================

        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId) ||
            x.TenantId == CurrentTenantId);


        // =========================
        // Student → Tenant
        // =========================

        entity.HasOne(x => x.Tenant)
            .WithMany(x => x.Students)
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        // =========================
        // Student → User
        // Required one-to-one
        // =========================

        entity.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Student>(x => x.UserId)
            .HasPrincipalKey<User>(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);


        // =========================
        // Student → Class
        // =========================
        //
        // No direct relationship.
        //
        // Student → StudentEnrollment → Class
        //
        // This allows enrollment history.
        // =========================


        // =========================
        // Indexes
        // =========================

        // Student number unique per tenant
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.StudentNo
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_Student_Tenant_StudentNo");


        // One Student per User
        entity.HasIndex(x => x.UserId)
            .IsUnique()
            .HasDatabaseName(
                "IX_Student_UserId");
    });
}
private void ConfigureRegistrar(ModelBuilder builder)
{
    builder.Entity<Registrar>(entity =>
    {
        entity.HasKey(x => x.RegistrarId);

        entity.Property(x => x.EmployeeNo)
            .IsRequired()
            .HasMaxLength(30);

        entity.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(x => x.Email)
            .HasMaxLength(150);

        entity.Property(x => x.Phone)
            .HasMaxLength(20);

        entity.Property(x => x.Status)
            .IsRequired();

        entity.Property(x => x.CreatedAt)
            .IsRequired();

        // Tenant isolation
        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId) ||
            x.TenantId == CurrentTenantId);

        // Registrar -> Tenant
        entity.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);

        // Registrar -> User
        entity.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Registrar>(x => x.UserId)
            .HasPrincipalKey<User>(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);

        // Registrar -> Department
        entity.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Employee number unique per tenant
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.EmployeeNo
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_Registrar_Tenant_EmployeeNo");

        // One Identity user can belong to one Registrar
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.UserId
        })
        .IsUnique()
        .HasDatabaseName(
            "IX_Registrar_Tenant_User");

        // Registrar email lookup
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.Email
        })
        .HasDatabaseName(
            "IX_Registrar_Tenant_Email");

        // Department lookup
        entity.HasIndex(x => new
        {
            x.TenantId,
            x.DepartmentId
        })
        .HasDatabaseName(
            "IX_Registrar_Tenant_Department");
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
    private void ConfigureStudentGuardian(
        ModelBuilder builder)
{
    builder.Entity<StudentGuardian>(entity =>
    {
        entity.HasKey(x => x.StudentGuardianId);


        entity.HasQueryFilter(x =>
            string.IsNullOrWhiteSpace(CurrentTenantId)
            || x.TenantId == CurrentTenantId);



        entity.HasOne(x => x.Student)
            .WithMany(x => x.StudentGuardians)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);



        entity.HasOne(x => x.Guardian)
            .WithMany(x => x.StudentGuardians)
            .HasForeignKey(x => x.GuardianId)
            .OnDelete(DeleteBehavior.Restrict);



        entity.HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Restrict);


        entity.HasIndex(x => new
        {
            x.TenantId,
            x.StudentId,
            x.GuardianId
        })
        .IsUnique();



        entity.HasIndex(x => new
        {
            x.TenantId,
            x.StudentId
        });



        entity.HasIndex(x => new
        {
            x.TenantId,
            x.GuardianId
        });
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
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedNever()
            .HasMaxLength(100);


        entity.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);


        entity.Property(x => x.Subdomain)
            .IsRequired()
            .HasMaxLength(100);


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