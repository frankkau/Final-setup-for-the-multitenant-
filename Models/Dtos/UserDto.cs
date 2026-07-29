namespace CouncilRevenueCollection.Models.Dtos;

public record CreateUserDto(string Email,string FirstName, string LastName, string Password, List<string> Roles);
public record UpdateUserDto(string Email, string FirstName, string LastName, List<string> Roles);
public record UserResponseDto(string Id, string FirstName, string LastName, string TenantId, string Email, List<string> Roles);


public record ResetPasswordDto(
    string NewPassword
);
public class UsersResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public List<string> Roles { get; set; } = new();
}