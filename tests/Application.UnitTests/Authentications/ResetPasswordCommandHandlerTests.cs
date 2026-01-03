#nullable enable
using Application.Authentications.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.UnitTests.Authentications;

/// <summary>
/// Contains unit tests for <see cref="ResetPasswordCommandHandler"/>.
/// </summary>
public class ResetPasswordCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsFailure_WhenUserMissing()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new ResetPasswordCommandHandler(context, new TestPasswordHasherService(), new TestClockService());

        var result = await handler.Handle(new ResetPasswordCommand
        {
            Email = "missing@example.com",
            ResetToken = "token",
            NewPassword = "newpass!"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User not found.", result.Message);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenTokenInvalid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = new User
        {
            Username = "user",
            NormalizedUsername = "USER",
            Email = "user@example.com",
            NormalizedEmail = "USER@EXAMPLE.COM",
            PasswordResetToken = "correct-token",
            PasswordResetTokenExpiresAt = Instant.FromUtc(2025, 1, 1, 1, 0)
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new ResetPasswordCommandHandler(context, new TestPasswordHasherService(), new TestClockService());

        var result = await handler.Handle(new ResetPasswordCommand
        {
            Email = "user@example.com",
            ResetToken = "wrong-token",
            NewPassword = "newpass!"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Reset token is invalid.", result.Message);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenTokenExpired()
    {
        await using var context = TestDbContextFactory.Create();
        var user = new User
        {
            Username = "user",
            NormalizedUsername = "USER",
            Email = "user@example.com",
            NormalizedEmail = "USER@EXAMPLE.COM",
            PasswordResetToken = "token",
            PasswordResetTokenExpiresAt = Instant.FromUtc(2024, 12, 31, 23, 0)
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var clock = new TestClockService(Instant.FromUtc(2025, 1, 1, 0, 0));
        var handler = new ResetPasswordCommandHandler(context, new TestPasswordHasherService(), clock);

        var result = await handler.Handle(new ResetPasswordCommand
        {
            Email = "user@example.com",
            ResetToken = "token",
            NewPassword = "newpass!"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Reset token has expired.", result.Message);
    }

    [Fact]
    public async Task Handle_ResetsPassword_WhenTokenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = new User
        {
            Username = "user",
            NormalizedUsername = "USER",
            Email = "user@example.com",
            NormalizedEmail = "USER@EXAMPLE.COM",
            PasswordResetToken = "token",
            PasswordResetTokenExpiresAt = Instant.FromUtc(2025, 1, 2, 0, 0)
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new ResetPasswordCommandHandler(context, new TestPasswordHasherService(), new TestClockService());

        var result = await handler.Handle(new ResetPasswordCommand
        {
            Email = "user@example.com",
            ResetToken = "token",
            NewPassword = "newpass!"
        }, CancellationToken.None);

        Assert.True(result.Success);

        var updatedUser = await context.Users.SingleAsync();
        Assert.Equal("hashed::newpass!", updatedUser.PasswordHash);
        Assert.Null(updatedUser.PasswordResetToken);
        Assert.Null(updatedUser.PasswordResetTokenExpiresAt);
        Assert.Equal(0, updatedUser.AccessFailedCount);
        Assert.False(updatedUser.IsLocked);
    }
}
