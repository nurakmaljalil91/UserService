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
    public DbSet<User> Users => Set<User>();

    /// <inheritdoc />
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    /// <inheritdoc />
    public DbSet<Session> Sessions => Set<Session>();

    /// <inheritdoc />
    public DbSet<ContactMethod> ContactMethods => Set<ContactMethod>();

    /// <inheritdoc />
    public DbSet<Address> Addresses => Set<Address>();

    /// <inheritdoc />
    public DbSet<LoginAttempt> LoginAttempts => Set<LoginAttempt>();

    /// <inheritdoc />
    public DbSet<Consent> Consents => Set<Consent>();

    /// <inheritdoc />
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();

    /// <inheritdoc />
    public DbSet<Role> Roles => Set<Role>();

    /// <inheritdoc />
    public DbSet<Permission> Permissions => Set<Permission>();

    /// <inheritdoc />
    public DbSet<Group> Groups => Set<Group>();

    /// <inheritdoc />
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    /// <inheritdoc />
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    /// <inheritdoc />
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();

    /// <inheritdoc />
    public DbSet<GroupRole> GroupRoles => Set<GroupRole>();

    /// <inheritdoc />
    public DbSet<ExternalIdentity> ExternalIdentities => Set<ExternalIdentity>();

    /// <inheritdoc />
    public DbSet<ExternalToken> ExternalTokens => Set<ExternalToken>();

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
