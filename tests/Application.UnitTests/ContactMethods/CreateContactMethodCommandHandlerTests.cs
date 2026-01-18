#nullable enable
using Application.ContactMethods.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.ContactMethods;

/// <summary>
/// Unit tests for <see cref="CreateContactMethodCommandHandler"/>.
/// </summary>
public class CreateContactMethodCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler creates a contact method when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_CreatesContactMethod_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateContactMethodCommandHandler(context);

        var result = await handler.Handle(new CreateContactMethodCommand
        {
            UserId = user.Id,
            Type = " email ",
            Value = " User@Example.com ",
            IsVerified = true,
            IsPrimary = true
        }, CancellationToken.None);

        Assert.True(result.Success);

        var contactMethod = await context.ContactMethods.SingleAsync();
        Assert.Equal(user.Id, contactMethod.UserId);
        Assert.Equal("email", contactMethod.Type);
        Assert.Equal("User@Example.com", contactMethod.Value);
        Assert.Equal("USER@EXAMPLE.COM", contactMethod.NormalizedValue);
        Assert.True(contactMethod.IsVerified);
        Assert.True(contactMethod.IsPrimary);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserMissing_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new CreateContactMethodCommandHandler(context);

        var result = await handler.Handle(new CreateContactMethodCommand
        {
            UserId = Guid.NewGuid(),
            Type = "email",
            Value = "user@example.com"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User does not exist.", result.Message);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the contact method already exists.
    /// </summary>
    [Fact]
    public async Task Handle_WhenContactMethodExists_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        context.ContactMethods.Add(new ContactMethod
        {
            UserId = user.Id,
            Type = "email",
            Value = "user@example.com",
            NormalizedValue = "USER@EXAMPLE.COM"
        });
        await context.SaveChangesAsync();

        var handler = new CreateContactMethodCommandHandler(context);

        var result = await handler.Handle(new CreateContactMethodCommand
        {
            UserId = user.Id,
            Type = "email",
            Value = "user@example.com"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Contact method already exists.", result.Message);
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
