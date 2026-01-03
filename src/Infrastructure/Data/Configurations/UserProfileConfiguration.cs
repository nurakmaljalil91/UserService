using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Provides the Entity Framework configuration for the <see cref="UserProfile"/> entity.
/// </summary>
public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    /// <summary>
    /// Configures the <see cref="UserProfile"/> entity type.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.Property(p => p.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.Property(p => p.DisplayName)
            .HasMaxLength(150);

        builder.Property(p => p.FirstName)
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .HasMaxLength(100);

        builder.Property(p => p.IdentityCardNumber)
            .HasMaxLength(50);

        builder.Property(p => p.PassportNumber)
            .HasMaxLength(50);

        builder.Property(p => p.DateOfBirth);

        builder.Property(p => p.BirthPlace)
            .HasMaxLength(150);

        builder.Property(p => p.ShoeSize)
            .HasMaxLength(20);

        builder.Property(p => p.ClothingSize)
            .HasMaxLength(20);

        builder.Property(p => p.WaistSize)
            .HasMaxLength(20);

        builder.Property(p => p.Bio)
            .HasMaxLength(512);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(2048);

        builder.Property(p => p.Tag)
            .HasMaxLength(100);

        builder.Property(p => p.BloodType)
            .HasMaxLength(10);

        builder.HasIndex(p => p.UserId).IsUnique();

        builder.HasOne(p => p.User)
            .WithOne(u => u.Profile)
            .HasForeignKey<UserProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
