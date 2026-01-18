#nullable enable
using Application.Addresses.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Addresses;

/// <summary>
/// Unit tests for <see cref="CreateAddressCommandHandler"/>.
/// </summary>
public class CreateAddressCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler creates an address when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_CreatesAddress_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateAddressCommandHandler(context);

        var result = await handler.Handle(new CreateAddressCommand
        {
            UserId = user.Id,
            Label = " Home ",
            Type = "home",
            Line1 = " 123 Main ",
            City = " Metro ",
            IsDefault = true
        }, CancellationToken.None);

        Assert.True(result.Success);

        var address = await context.Addresses.SingleAsync();
        Assert.Equal(user.Id, address.UserId);
        Assert.Equal("Home", address.Label);
        Assert.Equal("home", address.Type);
        Assert.Equal("123 Main", address.Line1);
        Assert.Equal("Metro", address.City);
        Assert.True(address.IsDefault);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserMissing_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new CreateAddressCommandHandler(context);

        var result = await handler.Handle(new CreateAddressCommand
        {
            UserId = Guid.NewGuid(),
            Line1 = "123 Main"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User does not exist.", result.Message);
    }

    /// <summary>
    /// Creates a valid user entity for test scenarios.
    /// </summary>
    /// <returns>A configured <see cref="User"/> entity.</returns>
    private static User CreateUser()
    {
        var unique = Guid.NewGuid().ToString("N");
        return new User
        {
            Username = $"user-{unique}",
            NormalizedUsername = $"USER-{unique}".ToUpperInvariant(),
            Email = $"user-{unique}@example.com",
            NormalizedEmail = $"USER-{unique}@EXAMPLE.COM",
            PasswordHash = "hashed",
            EmailConfirm = false,
            PhoneNumberConfirm = false,
            TwoFactorEnabled = false,
            AccessFailedCount = 0,
            IsLocked = false,
            IsDeleted = false
        };
    }
}
