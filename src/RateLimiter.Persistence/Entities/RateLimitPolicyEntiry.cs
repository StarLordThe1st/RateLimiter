namespace RateLimiter.Persistence.Entities;

public sealed class RateLimitPolicyEntity
{
    public Guid Id { get; set; }

    public string SubjectPattern { get; set; } = null!;
    public string ResourcePattern { get; set; } = null!;

    public double Capacity { get; set; }
    public double RefillRatePerSecond { get; set; }

    public int Priority { get; set; }
    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
