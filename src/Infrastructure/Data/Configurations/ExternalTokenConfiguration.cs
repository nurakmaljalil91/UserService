using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Provides the Entity Framework configuration for the <see cref="ExternalToken"/> entity.
/// </summary>
public class ExternalTokenConfiguration : IEntityTypeConfiguration<ExternalToken>
{
    /// <summary>
    /// Configures the <see cref="ExternalToken"/> entity type.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<ExternalToken> builder)
    {
        builder.Property(e => e.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.Provider)
            .HasConversion(
                provider => provider.Value,
                value => ExternalProvider.From(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.AccessToken)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(e => e.RefreshToken)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(e => e.ExpiresAt)
            .IsRequired();

        builder.Property(e => e.Scopes)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.HasIndex(e => new { e.UserId, e.Provider })
            .IsUnique();
    }
}
