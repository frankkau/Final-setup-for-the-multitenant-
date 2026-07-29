using CouncilRevenueCollection.Common;
using Microsoft.AspNetCore.Identity;

namespace CouncilRevenueCollection.Models.Entity;



public class User : IdentityUser<string>, IMustHaveTenant
{
    public string TenantId { get; set; } = default!;
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        = new List<RefreshToken>();
}


// public class RefreshToken : IMustHaveTenant
// {
//     public Guid Id { get; set; }
//     public string TenantId { get; set; } = default!;
//     public string UserId { get; set; } = default!;
//     public virtual User User { get; set; } = null!;
//     public string Token { get; set; } = default!;
//     public DateTime ExpiresAtUtc { get; set; }
//     public bool IsRevoked { get; set; }

    

// }