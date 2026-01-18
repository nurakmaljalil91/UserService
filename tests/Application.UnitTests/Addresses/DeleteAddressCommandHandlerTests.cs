#nullable enable
using Application.Addresses.Commands;
using Application.Common.Exceptions;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Addresses;

/// <summary>
/// Unit tests for <see cref="DeleteAddressCommandHandler"/>.
/// </summary>
public class DeleteAddressCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the address does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenAddressMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new DeleteAddressCommandHandler(context);

        var command = new DeleteAddressCommand
        {
            Id = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler deletes the address when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_DeletesAddress_WhenExists()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        var address = new Address
        {
            UserId = user.Id,
            Line1 = "123 Main"
        };
        context.Users.Add(user);
        context.Addresses.Add(address);
        await context.SaveChangesAsync();

        var handler = new DeleteAddressCommandHandler(context);

        var result = await handler.Handle(new DeleteAddressCommand
        {
            Id = address.Id
        }, CancellationToken.None);

        Assert.True(result.Success);
        var remaining = await context.Addresses.CountAsync();
        Assert.Equal(0, remaining);
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
