#nullable enable
using Application.ExternalLinks.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;

namespace Application.UnitTests.ExternalLinks;

/// <summary>
/// Contains unit tests for <see cref="StartExternalLinkCommandHandler"/>.
/// </summary>
public sealed class StartExternalLinkCommandHandlerTests
{
    /// <summary>
    /// Tests that a valid user receives an authorization URL.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsAuthorizationUrl_WhenUserIsValid()
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

        var testUser = new TestUser { Username = "user" };
        var stateService = new TestExternalLinkStateService();
        var oauthService = new TestGoogleOAuthService();
        var handler = new StartExternalLinkCommandHandler(context, testUser, stateService, oauthService);

        var result = await handler.Handle(new StartExternalLinkCommand { Provider = "google" }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Contains("state=", result.Data!.AuthorizationUrl);
        Assert.Equal("google", result.Data!.Provider);
    }

    /// <summary>
    /// Tests that missing authentication returns a failure response.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsFailure_WhenUserIsNotAuthenticated()
    {
        await using var context = TestDbContextFactory.Create();
        var testUser = new TestUser { Username = null };
        var handler = new StartExternalLinkCommandHandler(
            context,
            testUser,
            new TestExternalLinkStateService(),
            new TestGoogleOAuthService());

        var result = await handler.Handle(new StartExternalLinkCommand { Provider = "google" }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User is not authenticated.", result.Message);
    }
}
