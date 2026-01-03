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
    /// Saves all changes made in this context to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
