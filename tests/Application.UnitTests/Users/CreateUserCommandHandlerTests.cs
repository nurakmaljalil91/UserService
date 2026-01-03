#nullable enable
using Application.Users.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Users;

/// <summary>
/// Contains unit tests for <see cref="CreateUserCommandHandler"/>.
/// </summary>
public class CreateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesUser_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new CreateUserCommandHandler(context, new TestPasswordHasherService());

        var result = await handler.Handle(new CreateUserCommand
        {
            Username = "user",
            Email = "user@example.com",
            PhoneNumber = "+12025550123",
            Password = "pass123!"
        }, CancellationToken.None);

        Assert.True(result.Success);

        var user = await context.Users.SingleAsync();
        Assert.Equal("user", user.Username);
        Assert.Equal("USER", user.NormalizedUsername);
        Assert.Equal("user@example.com", user.Email);
        Assert.Equal("USER@EXAMPLE.COM", user.NormalizedEmail);
        Assert.Equal("hashed::pass123!", user.PasswordHash);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenEmailExists()
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

        var handler = new CreateUserCommandHandler(context, new TestPasswordHasherService());

        var result = await handler.Handle(new CreateUserCommand
        {
            Username = "user",
            Email = "existing@example.com",
            Password = "pass123!"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Username or email already exists.", result.Message);
    }
}
