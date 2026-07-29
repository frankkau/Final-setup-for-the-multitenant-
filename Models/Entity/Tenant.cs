namespace CouncilRevenueCollection;

public class Tenant
{
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Subdomain { get; set; } = default!;

    public string? CustomDomain { get; set; }

    public bool IsActive { get; set; }
}

public class TenantInfo
{
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Subdomain { get; set; } = default!;
}