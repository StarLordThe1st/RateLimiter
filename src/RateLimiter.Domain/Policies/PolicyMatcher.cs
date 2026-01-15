namespace RateLimiter.Domain.Policies;

public static class PolicyMatcher
{
    public static bool Matches(string pattern, string value)
    {
        if (pattern == "*")
            return true;

        return string.Equals(pattern, value, StringComparison.OrdinalIgnoreCase);
    }
}