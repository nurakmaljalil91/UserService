#nullable enable
using Application.Addresses.Commands;
using Application.Common.Exceptions;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Addresses;

/// <summary>
/// Unit tests for <see cref="UpdateAddressCommandHandler"/>.
/// </summary>
public class UpdateAddressCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the address does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenAddressMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new UpdateAddressCommandHandler(context);

        var command = new UpdateAddressCommand
        {
            Id = Guid.NewGuid(),
            Line1 = "Line"
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the line1 is empty.
    /// </summary>
    [Fact]
    public async Task Handle_WhenLine1Empty_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var address = CreateAddress(context);
        var handler = new UpdateAddressCommandHandler(context);

        var result = await handler.Handle(new UpdateAddressCommand
        {
            Id = address.Id,
            Line1 = " "
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Address line1 is required.", result.Message);
    }

    /// <summary>
    /// Ensures the handler updates the address when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_UpdatesAddress_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var address = CreateAddress(context);
        var handler = new UpdateAddressCommandHandler(context);

        var result = await handler.Handle(new UpdateAddressCommand
        {
            Id = address.Id,
            City = "Metro",
            IsDefault = true
        }, CancellationToken.None);

        Assert.True(result.Success);

        var updated = await context.Addresses.SingleAsync(a => a.Id == address.Id);
        Assert.Equal("Metro", updated.City);
        Assert.True(updated.IsDefault);
    }

    /// <summary>
    /// Creates a valid address for test scenarios.
    /// </summary>
    /// <param name="context">The test database context.</param>
    /// <returns>The created <see cref="Address"/> entity.</returns>
    private static Address CreateAddress(TestApplicationDbContext context)
    {
        var user = CreateUser();
        var address = new Address
        {
            UserId = user.Id,
            Line1 = "123 Main"
        };
        context.Users.Add(user);
        context.Addresses.Add(address);
        context.SaveChanges();
        return address;
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
