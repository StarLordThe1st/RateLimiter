namespace RateLimiter.Domain.Results;

public sealed record RateLimitDecision(bool Allowed, string PolicyId, double RemainingTokens, TimeSpan? RetryAfter){
    public static RateLimitDecision AllowUnlimited() => new(true, "none", double.PositiveInfinity, null);
}

