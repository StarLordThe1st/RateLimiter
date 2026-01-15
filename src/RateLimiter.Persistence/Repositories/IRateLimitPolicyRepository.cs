using RateLimiter.Domain.Policies;

namespace RateLimiter.Persistence.Repositories;

public interface IRateLimitPolicyRepository
{
    Task<IReadOnlyList<RateLimitPolicy>> GetActivePoliciesAsync(
        CancellationToken cancellationToken);
}
