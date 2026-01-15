using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateLimiter.Persistence.Entities;

namespace RateLimiter.Persistence.Db.Configurations;

public sealed class RateLimitPolicyEntityConfiguration
    : IEntityTypeConfiguration<RateLimitPolicyEntity>
{
    public void Configure(EntityTypeBuilder<RateLimitPolicyEntity> builder)
    {
        builder.ToTable("rate_limit_policies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SubjectPattern)
            .IsRequired();

        builder.Property(x => x.ResourcePattern)
            .IsRequired();

        builder.Property(x => x.Capacity)
            .IsRequired();

        builder.Property(x => x.RefillRatePerSecond)
            .IsRequired();

        builder.Property(x => x.Priority)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAtUtc)
            .HasDefaultValueSql("now()");

        builder.Property(x => x.UpdatedAtUtc)
            .HasDefaultValueSql("now()");

        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.Priority);
    }
}
