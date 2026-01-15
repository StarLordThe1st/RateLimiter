using RateLimiter.Domain.Buckets;
using Xunit;

public sealed class TokenBucketTests
{
    [Fact]
    public void Allows_Initial_request_when_bucket_is_full()
    {
        var bucket = new TokenBucket(capacity: 10, refillRatePerSecord: 1);
        var state = new TokenBucketState(10, DateTimeOffset.UtcNow);

        var (_, result) = bucket.TryConsume(state, cost: 5, DateTimeOffset.UtcNow);

        Assert.True(result.Allowed);
        Assert.Equal(5, result.RemainingTokens);
    }

    [Fact]
    public void Denies_request_when_insufficient_tokens()
    {
        var now = DateTimeOffset.UtcNow;

        var bucket = new TokenBucket(capacity: 5, refillRatePerSecord: 1);
        var state = new TokenBucketState(1, now);

        var (_, result) = bucket.TryConsume(state, 3, now);

        Assert.False(result.Allowed);
        Assert.NotNull(result.RetryAfter);
    }

    [Fact]
    public void Refills_tokens_over_time()
    {
        var start = DateTimeOffset.UtcNow;

        var bucket = new TokenBucket(capacity: 10, refillRatePerSecord: 1);
        var state = new TokenBucketState(0, start);

        var later = start.AddSeconds(5);

        var (_, result) = bucket.TryConsume(state, 3, later);

        Assert.True(result.Allowed);
        Assert.Equal(3, result.RemainingTokens);
    }
}