namespace CouncilRevenueCollection.Models.Dtos.StudentGuardian;

public class StudentGuardianDto
{
    public Guid StudentGuardianId { get; set; }


    public Guid StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;



    public Guid GuardianId { get; set; }

    public string GuardianName { get; set; } = string.Empty;



    public bool IsPrimary { get; set; }


    public bool CanPickup { get; set; }
}