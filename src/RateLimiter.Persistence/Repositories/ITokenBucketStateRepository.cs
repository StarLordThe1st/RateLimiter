using RateLimiter.Domain.Buckets;

namespace RateLimiter.Persistence.Repositories;

public interface ITokenBucketStateRepository
{
    Task<(TokenBucketState State, long Version)?> GetAsync(
        string subject,
        string resource,
        Guid policyId,
        CancellationToken cancellationToken);

    Task<bool> TryUpdateAsync(
        string subject,
        string resource,
        Guid policyId,
        TokenBucketState newState,
        long expectedVersion,
        CancellationToken cancellationToken);

    Task<bool> TryInsertAsync(
    string subject,
    string resource,
    Guid policyId,
    TokenBucketState initialState,
    CancellationToken cancellationToken);

}
