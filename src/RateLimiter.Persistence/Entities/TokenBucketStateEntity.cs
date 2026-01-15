namespace RateLimiter.Persistence.Entities;

public sealed class TokenBucketStateEntity
{
    public string SubjectId { get; set; } = null!;
    public string Resource { get; set; } = null!;
    public Guid PolicyId { get; set; }

    public double Tokens { get; set; }
    public DateTimeOffset LastRefillUtc { get; set; }

    public long Version { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
