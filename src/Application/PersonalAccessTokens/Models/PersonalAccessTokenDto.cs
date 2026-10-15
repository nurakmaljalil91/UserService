#nullable enable
using Domain.Entities;
using NodaTime;
using NodaTime.Text;

namespace Application.PersonalAccessTokens.Models;

/// <summary>
/// Metadata about a personal access token. Never contains the secret.
/// </summary>
public record PersonalAccessTokenDto
{
    private static readonly InstantPattern InstantFormat =
        InstantPattern.CreateWithInvariantCulture("yyyy-MM-dd'T'HH:mm:ss'Z'");

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonalAccessTokenDto"/> class.
    /// </summary>
    public PersonalAccessTokenDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonalAccessTokenDto"/> class from an entity.
    /// </summary>
    /// <param name="token">The token entity.</param>
    /// <param name="now">The current instant, used to compute <see cref="IsActive"/>.</param>
    public PersonalAccessTokenDto(PersonalAccessToken token, Instant now)
    {
        Id = token.Id;
        Name = token.Name;
        DisplayPrefix = token.DisplayPrefix;
        Scopes = token.Scopes.ToList();
        CreatedAt = InstantFormat.Format(token.CreatedDate);
        ExpiresAt = Format(token.ExpiresAt);
        LastUsedAt = Format(token.LastUsedAt);
        RevokedAt = Format(token.RevokedAt);
        IsActive = token.IsActive(now);
    }

    /// <summary>Gets the token identifier.</summary>
    public Guid Id { get; init; }

    /// <summary>Gets the token name.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Gets the leading characters of the secret, for recognition.</summary>
    public string DisplayPrefix { get; init; } = string.Empty;

    /// <summary>Gets the granted scopes.</summary>
    public IReadOnlyList<string> Scopes { get; init; } = [];

    /// <summary>Gets when the token was created (UTC, ISO 8601).</summary>
    public string? CreatedAt { get; init; }

    /// <summary>Gets when the token expires, or <c>null</c> when it never expires.</summary>
    public string? ExpiresAt { get; init; }

    /// <summary>Gets when the token was last exchanged.</summary>
    public string? LastUsedAt { get; init; }

    /// <summary>Gets when the token was revoked.</summary>
    public string? RevokedAt { get; init; }

    /// <summary>Gets a value indicating whether the token can still be exchanged.</summary>
    public bool IsActive { get; init; }

    private static string? Format(Instant? instant) => instant.HasValue ? InstantFormat.Format(instant.Value) : null;
}

/// <summary>
/// A newly created personal access token, including its secret. Returned only once, at creation.
/// </summary>
public sealed record CreatedPersonalAccessTokenDto : PersonalAccessTokenDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreatedPersonalAccessTokenDto"/> class.
    /// </summary>
    public CreatedPersonalAccessTokenDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreatedPersonalAccessTokenDto"/> class.
    /// </summary>
    /// <param name="token">The token entity.</param>
    /// <param name="now">The current instant.</param>
    /// <param name="secret">The token secret.</param>
    public CreatedPersonalAccessTokenDto(PersonalAccessToken token, Instant now, string secret)
        : base(token, now)
    {
        Token = secret;
    }

    /// <summary>Gets the token secret. It cannot be retrieved again.</summary>
    public string Token { get; init; } = string.Empty;
}
