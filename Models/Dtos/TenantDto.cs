namespace CouncilRevenueCollection.Models.Dtos;
// public record CreateTenantDto(string Name,  string AdminPassword, string Subdomain, string? CustomDomain, string AdminEmail, string? AdminFirstName, string? AdminLastName /*, existing fields */);
// public record CreateTenantDto(
//     string Name,
//     string Subdomain,
//     string AdminEmail,
//     string AdminPassword,
//     string? CustomDomain = null
// );
public class CreateTenantDto
{
     public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Subdomain { get; set; }
    public string? CustomDomain { get; set; }
    public required string AdminEmail { get; set; }
    public required string AdminPassword { get; set; }
    public required string AdminFirstName { get; set; }
    public required string AdminLastName { get; set; }
}
public record UpdateTenantDto(string Name, string CustomDomain, string Subdomain, bool IsActive);
public record TenantResponseDto(string Id, string Name, string Subdomain, string CustomDomain, bool IsActive);


public record TenantPublicDetailsDto(
    string Id, 
    string Name, 
    string Subdomain,
    string? CustomDomain, 
    string? LogoUrl, 
    string? PrimaryColor
);