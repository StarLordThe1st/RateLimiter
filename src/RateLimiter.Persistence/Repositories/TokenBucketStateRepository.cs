using Microsoft.EntityFrameworkCore;
using RateLimiter.Domain.Buckets;
using RateLimiter.Persistence.Db;
using RateLimiter.Persistence.Entities;

namespace RateLimiter.Persistence.Repositories;

public sealed class TokenBucketStateRepository : ITokenBucketStateRepository
{
    private readonly RateLimiterDbContext _db;

    public TokenBucketStateRepository(RateLimiterDbContext db)
    {
        _db = db;
    }

    public async Task<(TokenBucketState State, long Version)?> GetAsync(
        string subject,
        string resource,
        Guid policyId,
        CancellationToken cancellationToken)
    {
        var entity = await _db.TokenBucketStates
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.SubjectId == subject &&
                x.Resource == resource &&
                x.PolicyId == policyId,
                cancellationToken);

        if (entity is null)
            return null;

        var state = new TokenBucketState(
            entity.Tokens,
            entity.LastRefillUtc);

        return (state, entity.Version);
    }

    public async Task<bool> TryInsertAsync(
        string subject,
        string resource,
        Guid policyId,
        TokenBucketState initialState,
        CancellationToken cancellationToken)
    {
        var entity = new TokenBucketStateEntity
        {
            SubjectId = subject,
            Resource = resource,
            PolicyId = policyId,
            Tokens = initialState.Tokens,
            LastRefillUtc = initialState.LastRefillUtc,
            Version = 0,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        _db.TokenBucketStates.Add(entity);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    public async Task<bool> TryUpdateAsync(
        string subject,
        string resource,
        Guid policyId,
        TokenBucketState newState,
        long expectedVersion,
        CancellationToken cancellationToken)
    {
        var entity = new TokenBucketStateEntity
        {
            SubjectId = subject,
            Resource = resource,
            PolicyId = policyId,
            Tokens = newState.Tokens,
            LastRefillUtc = newState.LastRefillUtc,
            Version = expectedVersion + 1,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        _db.Attach(entity);
        _db.Entry(entity).Property(x => x.Version).OriginalValue = expectedVersion;
        _db.Entry(entity).State = EntityState.Modified;

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
    }
}
