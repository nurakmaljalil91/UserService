#nullable enable
using Application.Common.Exceptions;
using Application.ContactMethods.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.ContactMethods;

/// <summary>
/// Unit tests for <see cref="UpdateContactMethodCommandHandler"/>.
/// </summary>
public class UpdateContactMethodCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the contact method does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenContactMethodMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new UpdateContactMethodCommandHandler(context);

        var command = new UpdateContactMethodCommand
        {
            Id = Guid.NewGuid(),
            Type = "email"
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the contact method already exists.
    /// </summary>
    [Fact]
    public async Task Handle_WhenContactMethodExists_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        var existing = new ContactMethod
        {
            UserId = user.Id,
            Type = "email",
            Value = "user@example.com",
            NormalizedValue = "USER@EXAMPLE.COM"
        };
        var duplicate = new ContactMethod
        {
            UserId = user.Id,
            Type = "phone",
            Value = "123",
            NormalizedValue = "123"
        };
        context.Users.Add(user);
        context.ContactMethods.AddRange(existing, duplicate);
        await context.SaveChangesAsync();

        var handler = new UpdateContactMethodCommandHandler(context);

        var result = await handler.Handle(new UpdateContactMethodCommand
        {
            Id = existing.Id,
            Type = "phone",
            Value = "123"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Contact method already exists.", result.Message);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the value is empty.
    /// </summary>
    [Fact]
    public async Task Handle_WhenValueEmpty_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var contactMethod = CreateContactMethod(context);
        var handler = new UpdateContactMethodCommandHandler(context);

        var result = await handler.Handle(new UpdateContactMethodCommand
        {
            Id = contactMethod.Id,
            Value = " "
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Contact value is required.", result.Message);
    }

    /// <summary>
    /// Ensures the handler updates the contact method when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_UpdatesContactMethod_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var contactMethod = CreateContactMethod(context);
        var handler = new UpdateContactMethodCommandHandler(context);

        var result = await handler.Handle(new UpdateContactMethodCommand
        {
            Id = contactMethod.Id,
            Type = "phone",
            Value = " 123 ",
            IsVerified = true,
            IsPrimary = true
        }, CancellationToken.None);

        Assert.True(result.Success);

        var updated = await context.ContactMethods.SingleAsync(c => c.Id == contactMethod.Id);
        Assert.Equal("phone", updated.Type);
        Assert.Equal("123", updated.Value);
        Assert.Equal("123", updated.NormalizedValue);
        Assert.True(updated.IsVerified);
        Assert.True(updated.IsPrimary);
    }

    /// <summary>
    /// Creates a valid contact method for test scenarios.
    /// </summary>
    /// <param name="context">The test database context.</param>
    /// <returns>The created <see cref="ContactMethod"/> entity.</returns>
    private static ContactMethod CreateContactMethod(TestApplicationDbContext context)
    {
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
        context.SaveChanges();
        return contactMethod;
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
