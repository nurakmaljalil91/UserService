#nullable enable
using Application.Addresses.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Addresses;

/// <summary>
/// Unit tests for <see cref="GetAddressesQueryHandler"/>.
/// </summary>
public class GetAddressesQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler returns a paginated list of addresses.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsPaginatedAddresses()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        context.Addresses.AddRange(
            new Address { UserId = user.Id, Line1 = "123 Main" },
            new Address { UserId = user.Id, Line1 = "456 Side" });
        await context.SaveChangesAsync();

        var handler = new GetAddressesQueryHandler(context);

        var result = await handler.Handle(new GetAddressesQuery
        {
            Page = 1,
            Total = 10
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.TotalCount);
        Assert.Equal(2, await context.Addresses.CountAsync());
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
