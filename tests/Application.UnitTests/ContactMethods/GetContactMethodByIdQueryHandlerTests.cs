#nullable enable
using Application.Common.Exceptions;
using Application.ContactMethods.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;

namespace Application.UnitTests.ContactMethods;

/// <summary>
/// Unit tests for <see cref="GetContactMethodByIdQueryHandler"/>.
/// </summary>
public class GetContactMethodByIdQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the contact method does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenContactMethodMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new GetContactMethodByIdQueryHandler(context);

        var query = new GetContactMethodByIdQuery
        {
            Id = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(query, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns the contact method when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsContactMethod_WhenExists()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        var contactMethod = new ContactMethod
        {
            User = user,
            Type = "email",
            Value = "user@example.com",
            NormalizedValue = "USER@EXAMPLE.COM"
        };
        context.Users.Add(user);
        context.ContactMethods.Add(contactMethod);
        await context.SaveChangesAsync();

        var handler = new GetContactMethodByIdQueryHandler(context);

        var result = await handler.Handle(new GetContactMethodByIdQuery
        {
            Id = contactMethod.Id
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(contactMethod.Id, result.Data!.Id);
        Assert.Equal(user.Id, result.Data!.UserId);
        Assert.Equal("email", result.Data!.Type);
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
