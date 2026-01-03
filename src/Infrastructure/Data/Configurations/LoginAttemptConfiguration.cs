using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Provides the Entity Framework configuration for the <see cref="LoginAttempt"/> entity.
/// </summary>
public class LoginAttemptConfiguration : IEntityTypeConfiguration<LoginAttempt>
{
    /// <summary>
    /// Configures the <see cref="LoginAttempt"/> entity type.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<LoginAttempt> builder)
    {
        builder.Property(l => l.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        builder.Property(l => l.Identifier)
            .HasMaxLength(256);

        builder.Property(l => l.IpAddress)
            .HasMaxLength(64);

        builder.Property(l => l.UserAgent)
            .HasMaxLength(512);

        builder.Property(l => l.FailureReason)
            .HasMaxLength(256);

        builder.Property(l => l.IsSuccessful)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(l => l.AttemptedAt)
            .IsRequired();

        builder.HasIndex(l => l.UserId);
        builder.HasIndex(l => l.Identifier);
        builder.HasIndex(l => l.AttemptedAt);

        builder.HasOne(l => l.User)
            .WithMany(u => u.LoginAttempts)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
