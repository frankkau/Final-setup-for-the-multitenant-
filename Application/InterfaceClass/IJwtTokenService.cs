using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Entity;

namespace CouncilRevenueCollection.Application.InterfaceClass;
public interface IJwtTokenService
{
    Task<LoginResponseDto> GenerateTokenAsync(User user);
}