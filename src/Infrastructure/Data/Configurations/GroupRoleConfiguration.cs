using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Provides the Entity Framework configuration for the <see cref="GroupRole"/> entity.
/// </summary>
public class GroupRoleConfiguration : IEntityTypeConfiguration<GroupRole>
{
    /// <summary>
    /// Configures the <see cref="GroupRole"/> entity type.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<GroupRole> builder)
    {
        builder.HasKey(gr => new { gr.GroupId, gr.RoleId });

        builder.HasOne(gr => gr.Group)
            .WithMany(g => g.GroupRoles)
            .HasForeignKey(gr => gr.GroupId)
            .IsRequired();

        builder.HasOne(gr => gr.Role)
            .WithMany(r => r.GroupRoles)
            .HasForeignKey(gr => gr.RoleId)
            .IsRequired();
    }
}
