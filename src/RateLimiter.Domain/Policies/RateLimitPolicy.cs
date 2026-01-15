namespace RateLimiter.Domain.Policies;

public sealed record RateLimitPolicy(Guid Id, string SubjectPattern, string ResourcePattern, double Capacity, double RefillRatePerSecond, int Priority);