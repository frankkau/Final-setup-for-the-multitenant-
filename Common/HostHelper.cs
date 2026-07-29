namespace CouncilRevenueCollection;

public static class HostHelper
{
    public static string? GetSubdomain(string host)
    {
        var parts = host.Split('.');

        if (parts.Length < 3)
            return null;

        return parts[0];
    }
}