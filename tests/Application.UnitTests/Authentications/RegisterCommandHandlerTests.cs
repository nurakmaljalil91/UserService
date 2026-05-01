#nullable enable
using Application.Authentications.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Authentications;

/// <summary>
/// Contains unit tests for <see cref="RegisterCommandHandler"/>.
/// </summary>
public class RegisterCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesUser_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var passwordHasher = new TestPasswordHasherService();
        var publisher = new TestNotificationRequestPublisher();
        var handler = new RegisterCommandHandler(context, passwordHasher, publisher);

        var command = new RegisterCommand
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "pass123!"
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Success);

        var user = await context.Users.SingleAsync();
        var role = await context.Roles.SingleAsync();
        var userRole = await context.UserRoles.SingleAsync();
        Assert.Equal("newuser", user.Username);
        Assert.Equal("NEWUSER", user.NormalizedUsername);
        Assert.Equal("newuser@example.com", user.Email);
        Assert.Equal("NEWUSER@EXAMPLE.COM", user.NormalizedEmail);
        Assert.Equal("hashed::pass123!", user.PasswordHash);
        Assert.Equal("User", role.Name);
        Assert.Equal("USER", role.NormalizedName);
        Assert.Equal(user.Id, userRole.UserId);
        Assert.Equal(role.Id, userRole.RoleId);

        var notification = Assert.Single(publisher.Notifications);
        Assert.Equal("UserService", notification.SourceService);
        Assert.Equal("UserRegistered", notification.SourceEventType);
        Assert.Equal(user.Id.ToString(), notification.SourceEventId);
        Assert.Equal("Welcome newuser!", notification.Title);
        Assert.Equal("Welcome newuser!", notification.Body);
        var recipient = Assert.Single(notification.Recipients);
        Assert.Equal(user.Id.ToString(), recipient.RecipientId);
        Assert.Equal("newuser", recipient.DisplayName);
    }

    [Fact]
    public async Task Handle_AssignsExistingUserRole_WhenUserRoleExists()
    {
        await using var context = TestDbContextFactory.Create();
        var role = new Role
        {
            Name = "User",
            NormalizedName = "USER",
            Description = "Standard user"
        };
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var handler = new RegisterCommandHandler(
            context,
            new TestPasswordHasherService(),
            new TestNotificationRequestPublisher());

        var result = await handler.Handle(new RegisterCommand
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "pass123!"
        }, CancellationToken.None);

        Assert.True(result.Success);

        var user = await context.Users.SingleAsync();
        var userRole = await context.UserRoles.SingleAsync();
        Assert.Single(context.Roles);
        Assert.Equal(user.Id, userRole.UserId);
        Assert.Equal(role.Id, userRole.RoleId);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenUsernameExists()
    {
        await using var context = TestDbContextFactory.Create();
        context.Users.Add(new User
        {
            Username = "existing",
            NormalizedUsername = "EXISTING",
            Email = "existing@example.com",
            NormalizedEmail = "EXISTING@EXAMPLE.COM"
        });
        await context.SaveChangesAsync();

        var publisher = new TestNotificationRequestPublisher();
        var handler = new RegisterCommandHandler(context, new TestPasswordHasherService(), publisher);

        var result = await handler.Handle(new RegisterCommand
        {
            Username = "existing",
            Email = "new@example.com",
            Password = "pass123!"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Username or email already exists.", result.Message);
        Assert.Empty(publisher.Notifications);
    }
}
