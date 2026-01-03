using System.Reflection.Emit;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Provides the Entity Framework configuration for the <see cref="TodoList"/> entity.
/// </summary>
public class TodoListConfiguration : IEntityTypeConfiguration<TodoList>
{
    /// <summary>
    /// Configures the <see cref="TodoList"/> entity type for Entity Framework.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<TodoList> builder)
    {

        builder
            .Navigation(x => x.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);


        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder
            .OwnsOne(b => b.Colour);
    }
}
