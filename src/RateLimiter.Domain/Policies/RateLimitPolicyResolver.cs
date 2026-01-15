using RateLimiter.Domain.Buckets;
using RateLimiter.Domain.Results;

namespace RateLimiter.Domain.Policies;

public sealed class RateLimitPolicyResolver
{
    public RateLimitDecision Resolve(IEnumerable<RateLimitPolicy> policies, string subject, string resource, int cost, TokenBucketState currentState, DateTimeOffset nowUtc)
    {
        var policy = policies.Where(p => PolicyMatcher.Matches(p.SubjectPattern, subject) && PolicyMatcher.Matches(p.ResourcePattern, resource))
                                                        .OrderByDescending(p => p.Priority)
                                                        .ThenByDescending(p => p.SubjectPattern != "*")
                                                        .ThenByDescending(p => p.ResourcePattern != "*")
                                                        .FirstOrDefault();

        if (policy is null)
        {
            return new RateLimitDecision(Allowed: true, PolicyId: "none", RemainingTokens: double.PositiveInfinity, RetryAfter: null);
        }

        var bucket = new TokenBucket(policy.Capacity, policy.RefillRatePerSecond);

        var (newState, result) = bucket.TryConsume(currentState, cost, nowUtc);

        return new RateLimitDecision(Allowed: result.Allowed, PolicyId: BuildPolicyId(policy), RemainingTokens: result.RemainingTokens, RetryAfter: result.RetryAfter);
    }

    private static string BuildPolicyId(RateLimitPolicy policy) => $"{policy.SubjectPattern}:{policy.ResourcePattern}:{policy.Priority}";
}