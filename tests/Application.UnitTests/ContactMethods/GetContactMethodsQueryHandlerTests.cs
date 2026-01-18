#nullable enable
using Application.ContactMethods.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.ContactMethods;

/// <summary>
/// Unit tests for <see cref="GetContactMethodsQueryHandler"/>.
/// </summary>
public class GetContactMethodsQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler returns a paginated list of contact methods.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsPaginatedContactMethods()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        context.ContactMethods.AddRange(
            new ContactMethod { UserId = user.Id, Type = "email", Value = "user@example.com", NormalizedValue = "USER@EXAMPLE.COM" },
            new ContactMethod { UserId = user.Id, Type = "phone", Value = "123", NormalizedValue = "123" });
        await context.SaveChangesAsync();

        var handler = new GetContactMethodsQueryHandler(context);

        var result = await handler.Handle(new GetContactMethodsQuery
        {
            Page = 1,
            Total = 10
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.TotalCount);
        Assert.Equal(2, await context.ContactMethods.CountAsync());
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
