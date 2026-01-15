#nullable enable
using Application.ExternalLinks.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Domain.ValueObjects;
using NodaTime;

namespace Application.UnitTests.ExternalLinks;

/// <summary>
/// Contains unit tests for <see cref="GetExternalLinksQueryHandler"/>.
/// </summary>
public sealed class GetExternalLinksQueryHandlerTests
{
    /// <summary>
    /// Tests that linked providers are returned for the current user.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsLinkedProviders_WhenLinked()
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

        context.ExternalIdentities.Add(new ExternalIdentity
        {
            UserId = user.Id,
            Provider = ExternalProvider.From("google"),
            SubjectId = ExternalSubjectId.From("subject"),
            LinkedAt = Instant.FromUtc(2025, 1, 1, 0, 0),
            Email = "user@example.com",
            DisplayName = "User Example"
        });
        await context.SaveChangesAsync();

        var handler = new GetExternalLinksQueryHandler(context, new TestUser { Username = "user" });

        var result = await handler.Handle(new GetExternalLinksQuery(), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Single(result.Data!);
        Assert.Equal("google", result.Data!.First().Provider);
    }
}
