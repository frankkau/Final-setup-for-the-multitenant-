namespace CouncilRevenueCollection.Models.Dtos;

public record AssignUserRolesDto(string UserId, List<string> Roles);
public record UserRolesResponseDto(string UserId, string Email, string TenantId, List<string> AssignedRoles);