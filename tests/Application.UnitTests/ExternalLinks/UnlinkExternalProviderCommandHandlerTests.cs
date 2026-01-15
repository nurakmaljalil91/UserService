#nullable enable
using Application.ExternalLinks.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.ExternalLinks;

/// <summary>
/// Contains unit tests for <see cref="UnlinkExternalProviderCommandHandler"/>.
/// </summary>
public sealed class UnlinkExternalProviderCommandHandlerTests
{
    /// <summary>
    /// Tests that unlinking removes external identity and tokens.
    /// </summary>
    [Fact]
    public async Task Handle_RemovesExternalLink_WhenLinked()
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
        context.ExternalIdentities.Add(new ExternalIdentity
        {
            UserId = user.Id,
            Provider = provider,
            SubjectId = ExternalSubjectId.From("subject"),
            LinkedAt = new TestClockService().Now
        });
        context.ExternalTokens.Add(new ExternalToken
        {
            UserId = user.Id,
            Provider = provider,
            AccessToken = "token",
            RefreshToken = "refresh",
            ExpiresAt = new TestClockService().Now,
            Scopes = "https://www.googleapis.com/auth/calendar",
            UpdatedAt = new TestClockService().Now
        });
        await context.SaveChangesAsync();

        var handler = new UnlinkExternalProviderCommandHandler(context, new TestUser { Username = "user" });

        var result = await handler.Handle(new UnlinkExternalProviderCommand { Provider = "google" }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Empty(await context.ExternalIdentities.ToListAsync());
        Assert.Empty(await context.ExternalTokens.ToListAsync());
    }
}
