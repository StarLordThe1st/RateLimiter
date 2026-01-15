namespace RateLimiter.Domain.Results;

public sealed record RateLimitDecision(bool Allowed, string PolicyId, double RemainingTokens, TimeSpan? RetryAfter);