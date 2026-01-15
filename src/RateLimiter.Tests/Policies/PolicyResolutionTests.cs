using RateLimiter.Domain.Buckets;
using RateLimiter.Domain.Policies;
using Xunit;

public sealed class PolicyResolutionTests
{
    [Fact]
    public void Exact_policy_beats_wildcard()
    {
        var policies = new[]
        {
            new RateLimitPolicy("*", "*", 10, 1, Priority: 1),
            new RateLimitPolicy("user1", "/orders", 5, 1, Priority: 1)
        };

        var resolver = new RateLimitPolicyResolver();
        var state = new TokenBucketState(5, DateTimeOffset.UtcNow);

        var decision = resolver.Resolve(
            policies,
            "user1",
            "/orders",
            cost: 1,
            state,
            DateTimeOffset.UtcNow);

        Assert.True(decision.Allowed);
        Assert.Contains("user1:/orders", decision.PolicyId);
    }

    [Fact]
    public void Higher_priority_policy_wins()
    {
        var policies = new[]
        {
            new RateLimitPolicy("user1", "*", 5, 1, Priority: 1),
            new RateLimitPolicy("user1", "*", 1, 1, Priority: 10)
        };

        var resolver = new RateLimitPolicyResolver();
        var state = new TokenBucketState(1, DateTimeOffset.UtcNow);

        var decision = resolver.Resolve(
            policies,
            "user1",
            "/any",
            cost: 1,
            state,
            DateTimeOffset.UtcNow);

        Assert.True(decision.Allowed);
        Assert.Contains(":10", decision.PolicyId);
    }

    [Fact]
    public void No_matching_policy_allows_request()
    {
        var resolver = new RateLimitPolicyResolver();
        var state = new TokenBucketState(0, DateTimeOffset.UtcNow);

        var decision = resolver.Resolve(
            Array.Empty<RateLimitPolicy>(),
            "anon",
            "/health",
            cost: 1,
            state,
            DateTimeOffset.UtcNow);

        Assert.True(decision.Allowed);
        Assert.Equal("none", decision.PolicyId);
    }
}
