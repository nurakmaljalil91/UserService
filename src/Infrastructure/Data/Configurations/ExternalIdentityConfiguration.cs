using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Provides the Entity Framework configuration for the <see cref="ExternalIdentity"/> entity.
/// </summary>
public class ExternalIdentityConfiguration : IEntityTypeConfiguration<ExternalIdentity>
{
    /// <summary>
    /// Configures the <see cref="ExternalIdentity"/> entity type.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<ExternalIdentity> builder)
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

        builder.Property(e => e.SubjectId)
            .HasConversion(
                subject => subject.Value,
                value => ExternalSubjectId.From(value))
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasMaxLength(256);

        builder.Property(e => e.DisplayName)
            .HasMaxLength(200);

        builder.Property(e => e.LinkedAt)
            .IsRequired();

        builder.HasIndex(e => new { e.UserId, e.Provider })
            .IsUnique();

        builder.HasIndex(e => new { e.Provider, e.SubjectId })
            .IsUnique();
    }
}
