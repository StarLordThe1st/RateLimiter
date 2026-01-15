using RateLimiter.Domain.Results;

namespace RateLimiter.Api.Application;

public interface IRateLimiterApplicationService
{
    Task<RateLimitDecision> CheckAsync(string subject, string resource, int cost, CancellationToken cancellationToken);
}