#nullable enable
using Application.Addresses.Queries;
using Application.Common.Exceptions;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;

namespace Application.UnitTests.Addresses;

/// <summary>
/// Unit tests for <see cref="GetAddressByIdQueryHandler"/>.
/// </summary>
public class GetAddressByIdQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the address does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenAddressMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new GetAddressByIdQueryHandler(context);

        var query = new GetAddressByIdQuery
        {
            Id = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(query, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns the address when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsAddress_WhenExists()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        // Link via navigation so EF sets the foreign key after key generation
        var address = new Address { User = user, Line1 = "123 Main" };
        context.Users.Add(user);
        context.Addresses.Add(address);
        await context.SaveChangesAsync();

        var handler = new GetAddressByIdQueryHandler(context);

        var result = await handler.Handle(new GetAddressByIdQuery
        {
            Id = address.Id
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(address.Id, result.Data!.Id);
        Assert.Equal(user.Id, result.Data!.UserId);
        Assert.Equal("123 Main", result.Data!.Line1);
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
