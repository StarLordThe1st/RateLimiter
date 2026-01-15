namespace RateLimiter.Domain.Buckets;

public sealed record TokenBucketState(double Tokens, DateTimeOffset LastRefillUtc);