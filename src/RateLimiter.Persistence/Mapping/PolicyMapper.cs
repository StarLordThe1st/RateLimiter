using RateLimiter.Domain.Policies;
using RateLimiter.Persistence.Entities;

namespace RateLimiter.Persistence.Mapping;

internal static class PolicyMapper
{
    public static RateLimitPolicy ToDomain(this RateLimitPolicyEntity entity)
        => new(
            Id: entity.Id,
            SubjectPattern: entity.SubjectPattern,
            ResourcePattern: entity.ResourcePattern,
            Capacity: entity.Capacity,
            RefillRatePerSecond: entity.RefillRatePerSecond,
            Priority: entity.Priority);
}
