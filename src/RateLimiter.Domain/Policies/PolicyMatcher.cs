namespace RateLimiter.Domain.Policies;

internal static class PolicyMatcher
{
    public static bool Matches(string pattern, string value)
    {
        if (pattern == "*")
            return true;

        return string.Equals(pattern, value, StringComparison.OrdinalIgnoreCase);
    }
}