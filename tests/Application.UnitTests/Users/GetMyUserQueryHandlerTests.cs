#nullable enable
using Application.Common.Exceptions;
using Application.UnitTests.TestInfrastructure;
using Application.Users.Queries;
using Domain.Entities;

namespace Application.UnitTests.Users;

/// <summary>
/// Unit tests for <see cref="GetMyUserQueryHandler"/>.
/// </summary>
public class GetMyUserQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the user identifier is missing.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserIdMissing_ThrowsUnauthorizedAccessException()
    {
        await using var context = TestDbContextFactory.Create();
        var user = new TestUser();
        var handler = new GetMyUserQueryHandler(context, user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new GetMyUserQuery(), CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler throws when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var user = new TestUser { UserId = userId };
        var handler = new GetMyUserQueryHandler(context, user);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new GetMyUserQuery(), CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns the current user when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsCurrentUser()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var user = new TestUser { UserId = userId };

        context.Users.Add(CreateUser(userId));
        await context.SaveChangesAsync();

        var handler = new GetMyUserQueryHandler(context, user);

        var result = await handler.Handle(new GetMyUserQuery(), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(userId, result.Data!.Id);
    }

    /// <summary>
    /// Creates a valid user entity for test scenarios.
    /// </summary>
    /// <param name="userId">The identifier to assign to the user.</param>
    /// <returns>A configured <see cref="User"/> entity.</returns>
    private static User CreateUser(Guid userId)
    {
        var unique = Guid.NewGuid().ToString("N");
        return new User
        {
            Id = userId,
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
