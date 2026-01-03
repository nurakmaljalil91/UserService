#nullable enable

using Application.Common.Interfaces;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Data.Interceptors;

/// <summary>
/// Intercepts EF Core SaveChanges operations to automatically set audit properties
/// (CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) on entities inheriting from <see cref="Domain.Common.BaseAuditableEntity"/>.
/// </summary>
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IUser _user;
    private readonly IClockService _clock;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditableEntityInterceptor"/> class.
    /// </summary>
    /// <param name="user">The current user context.</param>
    /// <param name="clock">The clock service for retrieving the current time.</param>
    public AuditableEntityInterceptor(
        IUser user,
        IClockService clock)
    {
        _user = user;
        _clock = clock;
    }

    /// <summary>
    /// Called asynchronously before EF Core saves changes to the database.
    /// Updates audit properties on entities inheriting from <see cref="Domain.Common.BaseAuditableEntity"/>.
    /// </summary>
    /// <param name="eventData">The event data containing the <see cref="DbContext"/>.</param>
    /// <param name="result">The interception result.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// A <see cref="ValueTask{InterceptionResult}"/> representing the asynchronous operation.
    /// </returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }


    /// <summary>
    /// Updates audit properties on entities inheriting from <see cref="Domain.Common.BaseAuditableEntity"/>.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> whose entities will be updated. If <c>null</c>, no action is taken.</param>
    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker
            .Entries<BaseAuditableEntity>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities());

        var utcNow = _clock.Now;
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = _user.Username ?? "Default";
                entry.Entity.CreatedDate = utcNow;
            }
            entry.Entity.UpdatedBy = _user.Username ?? "Default";
            entry.Entity.UpdatedDate = utcNow;
        }
    }
}

/// <summary>
/// Provides extension methods for <see cref="EntityEntry"/>.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Determines whether any owned entities referenced by the specified <see cref="EntityEntry"/> have changed.
    /// </summary>
    /// <param name="entry">The <see cref="EntityEntry"/> to check.</param>
    /// <returns>
    /// <c>true</c> if any owned entities have been added or modified; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
