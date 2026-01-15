#nullable enable
using Application.ExternalLinks.Models;

namespace Application.Common.Interfaces;

/// <summary>
/// Provides Google OAuth operations for external account linking.
/// </summary>
public interface IGoogleOAuthService
{
    /// <summary>
    /// Builds an authorization URL for initiating Google OAuth.
    /// </summary>
    /// <param name="state">The CSRF protection state value.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The authorization URL.</returns>
    Task<string> BuildAuthorizationUrlAsync(string state, CancellationToken cancellationToken);

    /// <summary>
    /// Exchanges an authorization code for tokens.
    /// </summary>
    /// <param name="code">The authorization code.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The token response.</returns>
    Task<ExternalOAuthToken> ExchangeCodeAsync(string code, CancellationToken cancellationToken);

    /// <summary>
    /// Refreshes an access token using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The token response.</returns>
    Task<ExternalOAuthToken> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the Google user profile associated with an access token.
    /// </summary>
    /// <param name="accessToken">The access token.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The external user profile.</returns>
    Task<ExternalOAuthUserProfile> GetUserProfileAsync(string accessToken, CancellationToken cancellationToken);
}
