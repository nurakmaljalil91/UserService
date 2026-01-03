using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

/// <summary>
/// Represents the Entity Framework database context for the application,
/// providing access to <see cref="TodoList"/> and <see cref="TodoItem"/> entities.
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class using the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the <see cref="ApplicationDbContext"/>.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <inheritdoc />
    public DbSet<TodoList> TodoLists => Set<TodoList>();

    /// <inheritdoc />
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    /// <inheritdoc />
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Configures the entity model for the context.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
