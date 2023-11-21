namespace DistributedBanking.Data;

public static class Extensions
{
    public static string NormalizeString(this string value)
    {
        return value.ToUpperInvariant();
    }
}