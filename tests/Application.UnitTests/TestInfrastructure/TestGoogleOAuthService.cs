#nullable enable
using Application.Common.Interfaces;
using Application.ExternalLinks.Models;

namespace Application.UnitTests.TestInfrastructure;

/// <summary>
/// Provides a test implementation of <see cref="IGoogleOAuthService"/>.
/// </summary>
public sealed class TestGoogleOAuthService : IGoogleOAuthService
{
    /// <summary>
    /// Gets or sets the authorization URL returned by the service.
    /// </summary>
    public string AuthorizationUrl { get; set; } = "https://accounts.google.com/o/oauth2/v2/auth";

    /// <summary>
    /// Gets or sets the token response returned by the service.
    /// </summary>
    public ExternalOAuthToken TokenResponse { get; set; } = new()
    {
        AccessToken = "access-token",
        RefreshToken = "refresh-token",
        ExpiresInSeconds = 3600,
        Scope = "https://www.googleapis.com/auth/calendar",
        TokenType = "Bearer"
    };

    /// <summary>
    /// Gets or sets the user profile returned by the service.
    /// </summary>
    public ExternalOAuthUserProfile UserProfile { get; set; } = new()
    {
        SubjectId = "subject",
        Email = "user@example.com",
        DisplayName = "User Example"
    };

    /// <inheritdoc />
    public Task<string> BuildAuthorizationUrlAsync(string state, CancellationToken cancellationToken)
    {
        return Task.FromResult($"{AuthorizationUrl}?state={state}");
    }

    /// <inheritdoc />
    public Task<ExternalOAuthToken> ExchangeCodeAsync(string code, CancellationToken cancellationToken)
    {
        return Task.FromResult(TokenResponse);
    }

    /// <inheritdoc />
    public Task<ExternalOAuthToken> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        return Task.FromResult(TokenResponse with { RefreshToken = refreshToken });
    }

    /// <inheritdoc />
    public Task<ExternalOAuthUserProfile> GetUserProfileAsync(string accessToken, CancellationToken cancellationToken)
    {
        return Task.FromResult(UserProfile);
    }
}
