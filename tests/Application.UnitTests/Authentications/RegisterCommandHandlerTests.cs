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
        var handler = new RegisterCommandHandler(context, passwordHasher);

        var command = new RegisterCommand
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "pass123!"
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Success);

        var user = await context.Users.SingleAsync();
        Assert.Equal("newuser", user.Username);
        Assert.Equal("NEWUSER", user.NormalizedUsername);
        Assert.Equal("newuser@example.com", user.Email);
        Assert.Equal("NEWUSER@EXAMPLE.COM", user.NormalizedEmail);
        Assert.Equal("hashed::pass123!", user.PasswordHash);
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

        var handler = new RegisterCommandHandler(context, new TestPasswordHasherService());

        var result = await handler.Handle(new RegisterCommand
        {
            Username = "existing",
            Email = "new@example.com",
            Password = "pass123!"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Username or email already exists.", result.Message);
    }
}
