using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

/// <summary>
/// Represents the application's database context, providing access to TodoLists and TodoItems.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Gets the set of <see cref="TodoList"/> entities.
    /// </summary>
    DbSet<TodoList> TodoLists { get; }

    /// <summary>
    /// Gets the set of <see cref="TodoItem"/> entities.
    /// </summary>
    DbSet<TodoItem> TodoItems { get; }

    /// <summary>
    /// Gets the set of <see cref="User"/> entities.
    /// </summary>
    DbSet<User> Users { get; }

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
    /// Saves all changes made in this context to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
