using CouncilRevenueCollection.Common;

namespace CouncilRevenueCollection.Models.Entity;

public class RefreshToken : IMustHaveTenant
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string UserId { get; set; } = default!;
    public User User { get; set; } = default!;

    // SHA-256 hash of the refresh token
    public string TokenHash { get; set; } = default!;

    // JWT JTI associated with this refresh token
    public string JwtId { get; set; } = default!;

    public string TenantId { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    // Hash of the refresh token that replaced this one
    public string? ReplacedByTokenHash { get; set; }

    public string CreatedByIp { get; set; } = default!;

    public string? RevokedByIp { get; set; }

    // Optimistic concurrency
    public byte[] RowVersion { get; set; } = default!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsRevoked => RevokedAt != null;

    public bool IsActive => !IsExpired && !IsRevoked;
}