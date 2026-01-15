#nullable enable
using Application.ExternalLinks.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.ExternalLinks;

/// <summary>
/// Contains unit tests for <see cref="CompleteExternalLinkCommandHandler"/>.
/// </summary>
public sealed class CompleteExternalLinkCommandHandlerTests
{
    /// <summary>
    /// Tests that completing a link creates external identity and token records.
    /// </summary>
    [Fact]
    public async Task Handle_CreatesExternalLink_WhenValid()
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

        var stateService = new TestExternalLinkStateService
        {
            UserId = user.Id,
            Provider = ExternalProvider.From("google"),
            State = "state-1"
        };
        var oauthService = new TestGoogleOAuthService();
        var tokenProtector = new TestExternalTokenProtector();
        var handler = new CompleteExternalLinkCommandHandler(
            context,
            stateService,
            oauthService,
            tokenProtector,
            new TestClockService());

        var result = await handler.Handle(new CompleteExternalLinkCommand
        {
            Provider = "google",
            Code = "code-1",
            State = "state-1"
        }, CancellationToken.None);

        Assert.True(result.Success);
        var identity = await context.ExternalIdentities.SingleAsync();
        Assert.Equal(user.Id, identity.UserId);
        Assert.Equal("google", identity.Provider.Value);
        Assert.Equal("subject", identity.SubjectId.Value);

        var token = await context.ExternalTokens.SingleAsync();
        Assert.StartsWith("protected::", token.AccessToken);
        Assert.StartsWith("protected::", token.RefreshToken);
    }

    /// <summary>
    /// Tests that existing refresh tokens are reused when the provider does not return one.
    /// </summary>
    [Fact]
    public async Task Handle_UsesExistingRefreshToken_WhenMissing()
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
        context.ExternalTokens.Add(new ExternalToken
        {
            UserId = user.Id,
            Provider = provider,
            AccessToken = "protected::old-access",
            RefreshToken = "protected::old-refresh",
            ExpiresAt = new TestClockService().Now,
            Scopes = "https://www.googleapis.com/auth/calendar",
            UpdatedAt = new TestClockService().Now
        });
        await context.SaveChangesAsync();

        var stateService = new TestExternalLinkStateService
        {
            UserId = user.Id,
            Provider = provider,
            State = "state-2"
        };
        var oauthService = new TestGoogleOAuthService
        {
            TokenResponse = new Application.ExternalLinks.Models.ExternalOAuthToken
            {
                AccessToken = "access-token",
                RefreshToken = null,
                ExpiresInSeconds = 3600,
                Scope = "https://www.googleapis.com/auth/calendar",
                TokenType = "Bearer"
            }
        };

        var handler = new CompleteExternalLinkCommandHandler(
            context,
            stateService,
            oauthService,
            new TestExternalTokenProtector(),
            new TestClockService());

        var result = await handler.Handle(new CompleteExternalLinkCommand
        {
            Provider = "google",
            Code = "code-2",
            State = "state-2"
        }, CancellationToken.None);

        Assert.True(result.Success);
        var token = await context.ExternalTokens.SingleAsync();
        Assert.Contains("old-refresh", token.RefreshToken);
    }
}
