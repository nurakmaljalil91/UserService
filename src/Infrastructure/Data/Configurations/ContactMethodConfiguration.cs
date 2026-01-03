using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Provides the Entity Framework configuration for the <see cref="ContactMethod"/> entity.
/// </summary>
public class ContactMethodConfiguration : IEntityTypeConfiguration<ContactMethod>
{
    /// <summary>
    /// Configures the <see cref="ContactMethod"/> entity type.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<ContactMethod> builder)
    {
        builder.Property(c => c.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.Type)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.Value)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(c => c.NormalizedValue)
            .HasMaxLength(256);

        builder.Property(c => c.IsVerified)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(c => c.IsPrimary)
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasIndex(c => new { c.UserId, c.Type, c.NormalizedValue }).IsUnique();
        builder.HasIndex(c => c.NormalizedValue);

        builder.HasOne(c => c.User)
            .WithMany(u => u.ContactMethods)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
