using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateLimiter.Persistence.Entities;

namespace RateLimiter.Persistence.Db.Configurations;

public sealed class TokenBucketStateEntityConfiguration
    : IEntityTypeConfiguration<TokenBucketStateEntity>
{
    public void Configure(EntityTypeBuilder<TokenBucketStateEntity> builder)
    {
        builder.ToTable("token_bucket_states");

        builder.HasKey(x => new
        {
            x.SubjectId,
            x.Resource,
            x.PolicyId
        });

        builder.Property(x => x.SubjectId)
            .IsRequired();

        builder.Property(x => x.Resource)
            .IsRequired();

        builder.Property(x => x.PolicyId)
            .IsRequired();

        builder.Property(x => x.Tokens)
            .IsRequired();

        builder.Property(x => x.LastRefillUtc)
            .IsRequired();

        builder.Property(x => x.Version)
            .IsConcurrencyToken()
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasDefaultValueSql("now()");
    }
}
