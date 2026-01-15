using RateLimiter.Domain.Buckets;
using RateLimiter.Persistence.Entities;

internal static class TokenBucketStateMapper
{
    public static TokenBucketState ToDomain(this TokenBucketStateEntity entity)
        => new(entity.Tokens, entity.LastRefillUtc);
}
