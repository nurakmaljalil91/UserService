#nullable enable
using Application.Consents.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.UnitTests.Consents;

/// <summary>
/// Unit tests for <see cref="GetConsentsQueryHandler"/>.
/// </summary>
public class GetConsentsQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler returns a paginated list of consents.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsPaginatedConsents()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        context.Consents.AddRange(
            new Consent { UserId = user.Id, Type = "terms", IsGranted = true, GrantedAt = Instant.FromUtc(2026, 1, 1, 0, 0) },
            new Consent { UserId = user.Id, Type = "privacy", IsGranted = false, GrantedAt = Instant.FromUtc(2026, 2, 1, 0, 0) });
        await context.SaveChangesAsync();

        var handler = new GetConsentsQueryHandler(context);

        var result = await handler.Handle(new GetConsentsQuery
        {
            Page = 1,
            Total = 10
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.TotalCount);
        Assert.Equal(2, await context.Consents.CountAsync());
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
