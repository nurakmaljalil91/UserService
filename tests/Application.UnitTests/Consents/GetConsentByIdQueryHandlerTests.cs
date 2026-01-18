#nullable enable
using Application.Common.Exceptions;
using Application.Consents.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using NodaTime;

namespace Application.UnitTests.Consents;

/// <summary>
/// Unit tests for <see cref="GetConsentByIdQueryHandler"/>.
/// </summary>
public class GetConsentByIdQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the consent does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenConsentMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new GetConsentByIdQueryHandler(context);

        var query = new GetConsentByIdQuery
        {
            Id = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(query, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns the consent when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsConsent_WhenExists()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        // Establish relationship via navigation to ensure FK is set correctly
        var consent = new Consent
        {
            User = user,
            Type = "terms",
            IsGranted = true,
            GrantedAt = Instant.FromUtc(2026, 1, 1, 0, 0)
        };
        context.Users.Add(user);
        context.Consents.Add(consent);
        await context.SaveChangesAsync();

        var handler = new GetConsentByIdQueryHandler(context);

        var result = await handler.Handle(new GetConsentByIdQuery
        {
            Id = consent.Id
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(consent.Id, result.Data!.Id);
        Assert.Equal(user.Id, result.Data!.UserId);
        Assert.Equal("terms", result.Data!.Type);
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
