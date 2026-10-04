#nullable enable
using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.UserPreferences;

/// <summary>
/// Rules for the user preference that stores the user's IANA time zone.
/// </summary>
public static class TimeZonePreference
{
    /// <summary>
    /// The preference key under which the user's time zone is stored.
    /// </summary>
    public const string Key = "timezone";

    /// <summary>
    /// The validation message returned when the value is not a known IANA time zone id.
    /// </summary>
    public const string InvalidValueMessage = "Time zone must be a valid IANA time zone id, for example 'Asia/Kuala_Lumpur'.";

    /// <summary>
    /// Determines whether the given key is the time zone preference key.
    /// </summary>
    /// <param name="key">The preference key.</param>
    /// <returns><c>true</c> when the key is the time zone key; otherwise <c>false</c>.</returns>
    public static bool IsTimeZoneKey(string? key)
        => string.Equals(key?.Trim(), Key, StringComparison.Ordinal);

    /// <summary>
    /// Determines whether the value is a known IANA time zone id in the bundled tz database.
    /// </summary>
    /// <param name="value">The time zone id.</param>
    /// <returns><c>true</c> when the value is a valid IANA time zone id; otherwise <c>false</c>.</returns>
    public static bool IsValidValue(string? value)
        => !string.IsNullOrWhiteSpace(value)
           && DateTimeZoneProviders.Tzdb.GetZoneOrNull(value.Trim()) != null;

    /// <summary>
    /// Gets the user's saved time zone id, or <c>null</c> when none is saved.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The saved time zone id, or <c>null</c>.</returns>
    public static Task<string?> GetForUserAsync(
        IApplicationDbContext context,
        Guid userId,
        CancellationToken cancellationToken)
        => context.UserPreferences
            .Where(p => p.UserId == userId && p.Key == Key)
            .Select(p => p.Value)
            .FirstOrDefaultAsync(cancellationToken);
}
