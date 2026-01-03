using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Provides the Entity Framework configuration for the <see cref="Consent"/> entity.
/// </summary>
public class ConsentConfiguration : IEntityTypeConfiguration<Consent>
{
    /// <summary>
    /// Configures the <see cref="Consent"/> entity type.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Consent> builder)
    {
        builder.Property(c => c.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.Type)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.IsGranted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(c => c.GrantedAt)
            .IsRequired();

        builder.Property(c => c.Version)
            .HasMaxLength(50);

        builder.Property(c => c.Source)
            .HasMaxLength(50);

        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.Type);

        builder.HasOne(c => c.User)
            .WithMany(u => u.Consents)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
