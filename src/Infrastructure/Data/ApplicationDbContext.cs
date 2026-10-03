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
/// providing access to user, role, group, session, and related identity entities.
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

    /// <inheritdoc />
    public DbSet<Education> Educations => Set<Education>();

    /// <inheritdoc />
    public DbSet<Skill> Skills => Set<Skill>();

    /// <inheritdoc />
    public DbSet<WorkExperience> WorkExperiences => Set<WorkExperience>();

    /// <inheritdoc />
    public DbSet<Project> Projects => Set<Project>();

    /// <inheritdoc />
    public DbSet<Language> Languages => Set<Language>();

    /// <inheritdoc />
    public async Task<bool> SaveSuccessfulLoginAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (Database.IsInMemory())
        {
            await SaveChangesAsync(cancellationToken);
            var user = await Users.FindAsync([userId], cancellationToken);
            if (user?.OnboardingStatus != UserOnboardingStatus.Pending)
            {
                return false;
            }

            user.OnboardingStatus = UserOnboardingStatus.Prompted;
            try
            {
                await SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
        await SaveChangesAsync(cancellationToken);

        var claimed = await Users
            .Where(user => user.Id == userId && user.OnboardingStatus == UserOnboardingStatus.Pending)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    user => user.OnboardingStatus,
                    UserOnboardingStatus.Prompted),
                cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        return claimed == 1;
    }

    /// <inheritdoc />
    public async Task<bool> TrySetOnboardingOutcomeAsync(
        Guid userId,
        UserOnboardingStatus outcome,
        CancellationToken cancellationToken)
    {
        if (Database.IsInMemory())
        {
            var user = await Users.FindAsync([userId], cancellationToken);
            if (user?.OnboardingStatus != UserOnboardingStatus.Prompted)
            {
                return false;
            }

            user.OnboardingStatus = outcome;
            try
            {
                await SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        return await Users
            .Where(user => user.Id == userId && user.OnboardingStatus == UserOnboardingStatus.Prompted)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(user => user.OnboardingStatus, outcome),
                cancellationToken) == 1;
    }

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
