#nullable enable
using Application.Common.Interfaces;
using Application.ExternalLinks.Models;

namespace IntegrationTests.TestInfrastructure;

/// <summary>
/// Provides a test implementation of <see cref="IGoogleOAuthService"/> for integration tests.
/// </summary>
public sealed class TestGoogleOAuthService : IGoogleOAuthService
{
    /// <inheritdoc />
    public Task<string> BuildAuthorizationUrlAsync(string state, CancellationToken cancellationToken)
    {
        return Task.FromResult($"https://accounts.google.com/o/oauth2/v2/auth?state={state}");
    }

    /// <inheritdoc />
    public Task<ExternalOAuthToken> ExchangeCodeAsync(string code, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ExternalOAuthToken
        {
            AccessToken = "integration-access-token",
            RefreshToken = "integration-refresh-token",
            ExpiresInSeconds = 3600,
            Scope = "https://www.googleapis.com/auth/calendar",
            TokenType = "Bearer"
        });
    }

    /// <inheritdoc />
    public Task<ExternalOAuthToken> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ExternalOAuthToken
        {
            AccessToken = "integration-access-token-refreshed",
            RefreshToken = refreshToken,
            ExpiresInSeconds = 3600,
            Scope = "https://www.googleapis.com/auth/calendar",
            TokenType = "Bearer"
        });
    }

    /// <inheritdoc />
    public Task<ExternalOAuthUserProfile> GetUserProfileAsync(string accessToken, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ExternalOAuthUserProfile
        {
            SubjectId = "integration-subject",
            Email = "integration@example.com",
            DisplayName = "Integration User"
        });
    }
}
