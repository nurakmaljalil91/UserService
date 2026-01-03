using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Provides the Entity Framework configuration for the <see cref="Domain.Entities.TodoItem"/> entity.
/// </summary>
public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    /// <summary>
    /// Configures the <see cref="TodoItem"/> entity type.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.Property(t => t.Title)
       .HasMaxLength(200)
       .IsRequired();

        builder.Property(x => x.ListId).IsRequired();

        builder.HasOne(x => x.List)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.ListId);
    }
}
