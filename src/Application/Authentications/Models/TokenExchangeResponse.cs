#nullable enable
namespace Application.Authentications.Models;

/// <summary>
/// A short-lived access token issued in exchange for a personal access token.
/// </summary>
/// <param name="Token">The access token (JWT).</param>
/// <param name="ExpiresAt">When the access token expires (UTC).</param>
/// <param name="Scopes">The scopes carried by the access token.</param>
public sealed record TokenExchangeResponse(string Token, DateTime ExpiresAt, IReadOnlyList<string> Scopes);
