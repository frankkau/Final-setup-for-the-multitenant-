namespace CouncilRevenueCollection.Common;

public interface IMustHaveTenant
{
    string TenantId { get; }
}