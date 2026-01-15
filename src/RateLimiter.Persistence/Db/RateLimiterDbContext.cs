using Microsoft.EntityFrameworkCore;
using RateLimiter.Persistence.Entities;
using RateLimiter.Persistence.Db.Configurations;

namespace RateLimiter.Persistence.Db;

public sealed class RateLimiterDbContext : DbContext
{
    public RateLimiterDbContext(DbContextOptions<RateLimiterDbContext> options) : base(options)
    {
    }

    public DbSet<RateLimitPolicyEntity> RateLimitPolicies => Set<RateLimitPolicyEntity>();
    public DbSet<TokenBucketStateEntity> TokenBucketStates => Set<TokenBucketStateEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new RateLimitPolicyEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TokenBucketStateEntityConfiguration());
    }
}
