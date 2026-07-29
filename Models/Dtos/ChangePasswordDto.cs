namespace CouncilRevenueCollection.Models.Dtos;

public sealed class ChangePasswordDto
{
    public required string CurrentPassword { get; set; }

    public required string NewPassword { get; set; }
}