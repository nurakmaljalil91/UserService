using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Provides the Entity Framework configuration for the <see cref="Session"/> entity.
/// </summary>
public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    /// <summary>
    /// Configures the <see cref="Session"/> entity type.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.Property(s => s.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.RefreshToken)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(s => s.ExpiresAt)
            .IsRequired();

        builder.Property(s => s.RevokedAt);

        builder.Property(s => s.IpAddress)
            .HasMaxLength(64);

        builder.Property(s => s.UserAgent)
            .HasMaxLength(512);

        builder.Property(s => s.DeviceName)
            .HasMaxLength(100);

        builder.Property(s => s.IsRevoked)
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasIndex(s => s.RefreshToken).IsUnique();
        builder.HasIndex(s => s.UserId);

        builder.HasOne(s => s.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
