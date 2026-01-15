using RateLimiter.Domain.Buckets;
using RateLimiter.Domain.Policies;
using RateLimiter.Domain.Results;
using RateLimiter.Persistence.Repositories;

namespace RateLimiter.Api.Application;

public sealed class RateLimiterApplicationService : IRateLimiterApplicationService
{
    private const int MaxRetries = 5;

    private readonly IRateLimitPolicyRepository _policyRepository;
    private readonly ITokenBucketStateRepository _tokenBucketStateRepository;
    private readonly RateLimitPolicyResolver _policyResolver;

    public RateLimiterApplicationService(IRateLimitPolicyRepository policyRepository, ITokenBucketStateRepository tokenBucketStateRepository)
    {
        _policyRepository = policyRepository;
        _tokenBucketStateRepository = tokenBucketStateRepository;
        _policyResolver = new RateLimitPolicyResolver();
    }

    public async Task<RateLimitDecision> CheckAsync(string subject, string resource, int cost, CancellationToken cancellationToken)
    {
        var policies = await _policyRepository.GetActivePoliciesAsync(cancellationToken);
        var now = DateTimeOffset.UtcNow;

        for (var attempt = 0; attempt < MaxRetries; attempt++)
        {
            var policy = ResolvePolicy(policies, subject, resource);

            if (policy is null)
            {
                return RateLimitDecision.AllowUnlimited();
            }

            var bucketKey = await _tokenBucketStateRepository.GetAsync(subject, resource, policy.Id, cancellationToken);

            TokenBucketState state;
            long version;

            if (bucketKey is null)
            {
                state = new TokenBucketState(policy.Capacity, now);

                var inserted = await _tokenBucketStateRepository.TryInsertAsync(subject, resource, policy.Id, state, cancellationToken);

                if (!inserted)
                    continue;

                version = 0;
            }
            else
            {
                (state, version) = bucketKey.Value;
            }

            var bucket = new TokenBucket(policy.Capacity, policy.RefillRatePerSecond);
            var (newState, result) = bucket.TryConsume(state, cost, now);
            var updated = await _tokenBucketStateRepository.TryUpdateAsync(subject, resource, policy.Id, newState, version, cancellationToken);

            if (!updated)
                continue;

            return new RateLimitDecision(result.Allowed, policy.Id.ToString(), result.RemainingTokens, result.RetryAfter);
        }

        throw new InvalidOperationException("Failed to apply rate limit after max retries");
    }
    
    private static RateLimitPolicy? ResolvePolicy(IReadOnlyList<RateLimitPolicy> policies, string subject, string resource)
    {
        return policies.Where(p => PolicyMatcher.Matches(p.SubjectPattern, subject) && PolicyMatcher.Matches(p.ResourcePattern, resource))
                                                .OrderByDescending(p => p.Priority)
                                                .FirstOrDefault();
    }
}