#nullable enable
using Application.ExternalLinks.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Domain.ValueObjects;
using NodaTime;

namespace Application.UnitTests.ExternalLinks;

/// <summary>
/// Contains unit tests for <see cref="GetGoogleCalendarAccessTokenQueryHandler"/>.
/// </summary>
public sealed class GetGoogleCalendarAccessTokenQueryHandlerTests
{
    /// <summary>
    /// Tests that an expired token is refreshed and returned.
    /// </summary>
    [Fact]
    public async Task Handle_RefreshesToken_WhenExpired()
    {
        await using var context = TestDbContextFactory.Create();
        var user = new User
        {
            Username = "user",
            NormalizedUsername = "USER",
            Email = "user@example.com",
            NormalizedEmail = "USER@EXAMPLE.COM"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var provider = ExternalProvider.From("google");
        var tokenProtector = new TestExternalTokenProtector();
        context.ExternalTokens.Add(new ExternalToken
        {
            UserId = user.Id,
            Provider = provider,
            AccessToken = tokenProtector.Protect("old-access"),
            RefreshToken = tokenProtector.Protect("old-refresh"),
            ExpiresAt = Instant.FromUtc(2024, 1, 1, 0, 0),
            Scopes = "https://www.googleapis.com/auth/calendar",
            UpdatedAt = Instant.FromUtc(2024, 1, 1, 0, 0)
        });
        await context.SaveChangesAsync();

        var oauthService = new TestGoogleOAuthService
        {
            TokenResponse = new Application.ExternalLinks.Models.ExternalOAuthToken
            {
                AccessToken = "new-access",
                RefreshToken = "old-refresh",
                ExpiresInSeconds = 3600,
                Scope = "https://www.googleapis.com/auth/calendar",
                TokenType = "Bearer"
            }
        };
        var clock = new TestClockService(Instant.FromUtc(2025, 1, 1, 0, 0));
        var handler = new GetGoogleCalendarAccessTokenQueryHandler(context, oauthService, tokenProtector, clock);

        var result = await handler.Handle(new GetGoogleCalendarAccessTokenQuery { UserId = user.Id }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("new-access", result.Data!.AccessToken);
    }
}
