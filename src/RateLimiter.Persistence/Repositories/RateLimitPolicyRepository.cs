using Microsoft.EntityFrameworkCore;
using RateLimiter.Domain.Policies;
using RateLimiter.Persistence.Db;
using RateLimiter.Persistence.Mapping;

namespace RateLimiter.Persistence.Repositories;

public sealed class RateLimitPolicyRepository
    : IRateLimitPolicyRepository
{
    private readonly RateLimiterDbContext _db;

    public RateLimitPolicyRepository(RateLimiterDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<RateLimitPolicy>> GetActivePoliciesAsync(
        CancellationToken cancellationToken)
    {
        var entities = await _db.RateLimitPolicies
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.Priority)
            .ToListAsync(cancellationToken);

        return entities
            .Select(e => e.ToDomain())
            .ToList();
    }
}
