using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

/// <summary>
/// Represents the application's database context, providing access to TodoLists and TodoItems.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Gets the set of <see cref="User"/> entities.
    /// </summary>
    DbSet<User> Users { get; }

    /// <summary>
    /// Gets the set of <see cref="UserProfile"/> entities.
    /// </summary>
    DbSet<UserProfile> UserProfiles { get; }

    /// <summary>
    /// Gets the set of <see cref="Session"/> entities.
    /// </summary>
    DbSet<Session> Sessions { get; }

    /// <summary>
    /// Gets the set of <see cref="ContactMethod"/> entities.
    /// </summary>
    DbSet<ContactMethod> ContactMethods { get; }

    /// <summary>
    /// Gets the set of <see cref="Address"/> entities.
    /// </summary>
    DbSet<Address> Addresses { get; }

    /// <summary>
    /// Gets the set of <see cref="LoginAttempt"/> entities.
    /// </summary>
    DbSet<LoginAttempt> LoginAttempts { get; }

    /// <summary>
    /// Gets the set of <see cref="Consent"/> entities.
    /// </summary>
    DbSet<Consent> Consents { get; }

    /// <summary>
    /// Gets the set of <see cref="UserPreference"/> entities.
    /// </summary>
    DbSet<UserPreference> UserPreferences { get; }

    /// <summary>
    /// Gets the set of <see cref="Role"/> entities.
    /// </summary>
    DbSet<Role> Roles { get; }

    /// <summary>
    /// Gets the set of <see cref="Permission"/> entities.
    /// </summary>
    DbSet<Permission> Permissions { get; }

    /// <summary>
    /// Gets the set of <see cref="Group"/> entities.
    /// </summary>
    DbSet<Group> Groups { get; }

    /// <summary>
    /// Gets the set of <see cref="UserRole"/> entities.
    /// </summary>
    DbSet<UserRole> UserRoles { get; }

    /// <summary>
    /// Gets the set of <see cref="RolePermission"/> entities.
    /// </summary>
    DbSet<RolePermission> RolePermissions { get; }

    /// <summary>
    /// Gets the set of <see cref="UserGroup"/> entities.
    /// </summary>
    DbSet<UserGroup> UserGroups { get; }

    /// <summary>
    /// Gets the set of <see cref="GroupRole"/> entities.
    /// </summary>
    DbSet<GroupRole> GroupRoles { get; }

    /// <summary>
    /// Gets the set of <see cref="ExternalIdentity"/> entities.
    /// </summary>
    DbSet<ExternalIdentity> ExternalIdentities { get; }

    /// <summary>
    /// Gets the set of <see cref="ExternalToken"/> entities.
    /// </summary>
    DbSet<ExternalToken> ExternalTokens { get; }

    /// <summary>
    /// Saves all changes made in this context to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
