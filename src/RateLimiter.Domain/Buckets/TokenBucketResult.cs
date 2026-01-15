namespace RateLimiter.Domain.Buckets;

public sealed record TokenBucketResult(bool Allowed, double RemainingTokens, TimeSpan? RetryAfter);