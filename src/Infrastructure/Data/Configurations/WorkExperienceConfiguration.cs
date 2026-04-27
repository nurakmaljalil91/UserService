using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Provides the Entity Framework configuration for the <see cref="WorkExperience"/> entity.
/// </summary>
public class WorkExperienceConfiguration : IEntityTypeConfiguration<WorkExperience>
{
    /// <summary>
    /// Configures the <see cref="WorkExperience"/> entity type.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<WorkExperience> builder)
    {
        builder.Property(w => w.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        builder.Property(w => w.UserId)
            .IsRequired();

        builder.Property(w => w.Company)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(w => w.Position)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(w => w.StartDate)
            .IsRequired();

        builder.Property(w => w.EndDate);

        builder.Property(w => w.Description)
            .HasMaxLength(2000);

        builder.Property(w => w.Location)
            .HasMaxLength(200);

        builder.HasOne(w => w.User)
            .WithMany(u => u.WorkExperiences)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
