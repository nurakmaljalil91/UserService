#nullable enable
using Application.Common.Exceptions;
using Application.ContactMethods.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.ContactMethods;

/// <summary>
/// Unit tests for <see cref="DeleteContactMethodCommandHandler"/>.
/// </summary>
public class DeleteContactMethodCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the contact method does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenContactMethodMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new DeleteContactMethodCommandHandler(context);

        var command = new DeleteContactMethodCommand
        {
            Id = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler deletes the contact method when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_DeletesContactMethod_WhenExists()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        var contactMethod = new ContactMethod
        {
            UserId = user.Id,
            Type = "email",
            Value = "user@example.com",
            NormalizedValue = "USER@EXAMPLE.COM"
        };
        context.Users.Add(user);
        context.ContactMethods.Add(contactMethod);
        await context.SaveChangesAsync();

        var handler = new DeleteContactMethodCommandHandler(context);

        var result = await handler.Handle(new DeleteContactMethodCommand
        {
            Id = contactMethod.Id
        }, CancellationToken.None);

        Assert.True(result.Success);
        var remaining = await context.ContactMethods.CountAsync();
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
