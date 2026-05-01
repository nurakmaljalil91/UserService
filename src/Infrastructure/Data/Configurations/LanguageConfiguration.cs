using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Provides the Entity Framework configuration for the <see cref="Language"/> entity.
/// </summary>
public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    /// <summary>
    /// Configures the <see cref="Language"/> entity type.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.Property(l => l.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        builder.Property(l => l.UserId)
            .IsRequired();

        builder.Property(l => l.Name)
            .HasMaxLength(100);

        builder.Property(l => l.Level)
            .HasMaxLength(50);

        builder.HasIndex(l => l.UserId);

        builder.HasOne(l => l.User)
            .WithMany(u => u.Languages)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
