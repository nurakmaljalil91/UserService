#nullable enable
using Domain.Common;

namespace Domain.ValueObjects;

/// <summary>
/// A personal access token scope of the form <c>&lt;service&gt;:&lt;read|write&gt;</c>.
/// The <c>write</c> level of a service implies its <c>read</c> level.
/// </summary>
public sealed class TokenScope : ValueObject
{
    /// <summary>
    /// The services that personal access tokens can be scoped to.
    /// </summary>
    public static readonly IReadOnlyList<string> Services =
        ["users", "planner", "financial", "files", "notifications", "habits"];

    /// <summary>
    /// Every supported scope value, ordered by service then level.
    /// </summary>
    public static readonly IReadOnlyList<string> All =
        Services.SelectMany(service => new[] { $"{service}:read", $"{service}:write" }).ToList();

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenScope"/> class.
    /// </summary>
    /// <param name="value">The scope value, for example <c>planner:write</c>.</param>
    /// <exception cref="ArgumentException">The value is not a supported scope.</exception>
    public TokenScope(string value)
    {
        var normalized = value?.Trim().ToLowerInvariant() ?? string.Empty;
        if (!All.Contains(normalized))
        {
            throw new ArgumentException($"Unsupported scope '{value}'.", nameof(value));
        }

        Value = normalized;
    }

    /// <summary>
    /// Gets the normalized scope value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Determines whether the value is a supported scope.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><c>true</c> when the value is supported; otherwise <c>false</c>.</returns>
    public static bool IsValid(string? value)
        => value is not null && All.Contains(value.Trim().ToLowerInvariant());

    /// <summary>
    /// Normalizes, de-duplicates and expands scopes so that each <c>write</c> scope is accompanied by its <c>read</c> scope.
    /// </summary>
    /// <param name="scopes">The scopes to expand.</param>
    /// <returns>The expanded scopes in canonical order.</returns>
    /// <exception cref="ArgumentException">A value is not a supported scope.</exception>
    public static IReadOnlyList<string> Expand(IEnumerable<string> scopes)
    {
        var granted = scopes.Select(scope => new TokenScope(scope).Value).ToHashSet(StringComparer.Ordinal);

        foreach (var write in granted.Where(scope => scope.EndsWith(":write", StringComparison.Ordinal)).ToList())
        {
            granted.Add(write[..^"write".Length] + "read");
        }

        return All.Where(granted.Contains).ToList();
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
