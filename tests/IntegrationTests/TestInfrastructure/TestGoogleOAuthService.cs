#nullable enable
using Application.Common.Interfaces;
using Application.ExternalLinks.Models;

namespace IntegrationTests.TestInfrastructure;

/// <summary>
/// Provides a test implementation of <see cref="IGoogleOAuthService"/> for integration tests.
/// </summary>
public sealed class TestGoogleOAuthService : IGoogleOAuthService
{
    private const string CalendarScope = "https://www.googleapis.com/auth/calendar";

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
            AccessToken = $"integration-access-token-{code}",
            RefreshToken = $"integration-refresh-token-{code}",
            ExpiresInSeconds = 3600,
            Scope = CalendarScope,
            TokenType = "Bearer"
        });
    }

    /// <inheritdoc />
    public Task<ExternalOAuthToken> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ExternalOAuthToken
        {
            AccessToken = $"integration-access-token-refreshed-{refreshToken}",
            RefreshToken = refreshToken,
            ExpiresInSeconds = 3600,
            Scope = CalendarScope,
            TokenType = "Bearer"
        });
    }

    /// <inheritdoc />
    public Task<ExternalOAuthUserProfile> GetUserProfileAsync(string accessToken, CancellationToken cancellationToken)
    {
        var identifier = accessToken["integration-access-token-".Length..];

        return Task.FromResult(new ExternalOAuthUserProfile
        {
            SubjectId = $"integration-subject-{identifier}",
            Email = $"{identifier}@example.com",
            DisplayName = $"Integration User {identifier}"
        });
    }
}
