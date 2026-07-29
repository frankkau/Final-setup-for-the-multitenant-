namespace CouncilRevenueCollection.Models.Dtos;

public record CreateRoleDto(string RoleName, string Description );

public record RoleResponseDto(string Id, string Name, string TenantId);

public class RoleDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string TenantId { get; set; } = default!;
    public bool IsSystemRole { get; set; }
}