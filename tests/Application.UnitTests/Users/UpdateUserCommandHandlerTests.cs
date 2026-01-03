#nullable enable
using Application.Users.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Users;

/// <summary>
/// Contains unit tests for <see cref="UpdateUserCommandHandler"/>.
/// </summary>
public class UpdateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_UpdatesUser_WhenValid()
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

        var handler = new UpdateUserCommandHandler(context);

        var result = await handler.Handle(new UpdateUserCommand
        {
            Id = user.Id,
            Username = "updated",
            Email = "updated@example.com",
            PhoneNumber = "+12025550124",
            IsLocked = true
        }, CancellationToken.None);

        Assert.True(result.Success);

        var updated = await context.Users.SingleAsync();
        Assert.Equal("updated", updated.Username);
        Assert.Equal("UPDATED", updated.NormalizedUsername);
        Assert.Equal("updated@example.com", updated.Email);
        Assert.Equal("UPDATED@EXAMPLE.COM", updated.NormalizedEmail);
        Assert.Equal("+12025550124", updated.PhoneNumber);
        Assert.True(updated.IsLocked);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenUsernameExists()
    {
        await using var context = TestDbContextFactory.Create();
        var existing = new User
        {
            Username = "existing",
            NormalizedUsername = "EXISTING",
            Email = "existing@example.com",
            NormalizedEmail = "EXISTING@EXAMPLE.COM"
        };
        var target = new User
        {
            Username = "user",
            NormalizedUsername = "USER",
            Email = "user@example.com",
            NormalizedEmail = "USER@EXAMPLE.COM"
        };
        context.Users.AddRange(existing, target);
        await context.SaveChangesAsync();

        var handler = new UpdateUserCommandHandler(context);

        var result = await handler.Handle(new UpdateUserCommand
        {
            Id = target.Id,
            Username = "existing"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Username already exists.", result.Message);
    }
}
