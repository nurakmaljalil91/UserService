#nullable enable
using Application.Authentications.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.UnitTests.Authentications;

/// <summary>
/// Contains unit tests for <see cref="LoginCommandHandler"/>.
/// </summary>
public class LoginCommandHandlerTests
{
    /// <summary>
    /// Ensures a welcome notification is published on the user's first successful login.
    /// </summary>
    [Fact]
    public async Task Handle_PublishesWelcomeNotification_WhenFirstSuccessfulLogin()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var publisher = new TestNotificationRequestPublisher();
        var handler = CreateHandler(context, publisher);

        var result = await handler.Handle(new LoginCommand
        {
            Username = user.Username,
            Password = "pass123!"
        }, CancellationToken.None);

        Assert.True(result.Success);
        var notification = Assert.Single(publisher.Notifications);
        Assert.Equal("UserService", notification.SourceService);
        Assert.Equal("UserFirstLogin", notification.SourceEventType);
        Assert.Equal($"{user.Id}:first-login", notification.SourceEventId);
        Assert.Equal("Welcome firstuser!", notification.Title);
        Assert.Equal("Welcome firstuser!", notification.Body);
        var recipient = Assert.Single(notification.Recipients);
        Assert.Equal(user.Id.ToString(), recipient.RecipientId);
        Assert.Equal("firstuser", recipient.DisplayName);
    }

    /// <summary>
    /// Ensures a welcome notification is not published after the first successful login.
    /// </summary>
    [Fact]
    public async Task Handle_DoesNotPublishWelcomeNotification_WhenSuccessfulLoginAlreadyExists()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        context.LoginAttempts.Add(new LoginAttempt
        {
            UserId = user.Id,
            Identifier = user.Username,
            IsSuccessful = true,
            AttemptedAt = new TestClockService().Now
        });
        await context.SaveChangesAsync();
        var publisher = new TestNotificationRequestPublisher();
        var handler = CreateHandler(context, publisher);

        var result = await handler.Handle(new LoginCommand
        {
            Username = user.Username,
            Password = "pass123!"
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Empty(publisher.Notifications);
    }

    private static LoginCommandHandler CreateHandler(
        TestApplicationDbContext context,
        TestNotificationRequestPublisher publisher)
        => new(
            context,
            new TestPasswordHasherService(),
            new TestDateTime(),
            new TestClockService(),
            BuildConfiguration(),
            new TestRefreshTokenHasher(),
            publisher);

    private static IConfiguration BuildConfiguration()
        => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "Application.UnitTests",
                ["Jwt:Audience"] = "Application.UnitTests",
                ["Jwt:Key"] = "application-unit-tests-super-secret-key-1234567890",
                ["Jwt:ExpiryMinutes"] = "60",
                ["Jwt:RefreshTokenExpiryDays"] = "30"
            })
            .Build();

    private static User CreateUser()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "firstuser",
            NormalizedUsername = "FIRSTUSER",
            Email = "firstuser@example.com",
            NormalizedEmail = "FIRSTUSER@EXAMPLE.COM",
            IsDeleted = false,
            IsLocked = false
        };

        user.PasswordHash = new TestPasswordHasherService().HashPassword(user, "pass123!");
        return user;
    }
}
