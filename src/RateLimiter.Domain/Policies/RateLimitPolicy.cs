namespace RateLimiter.Domain.Policies;

public sealed record RateLimitPolicy(string SubjectPattern, string ResourcePattern, double Capacity, double RefillRatePerSecond, int Priority);