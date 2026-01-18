#nullable enable
using Application.Common.Exceptions;
using Application.Consents.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.UnitTests.Consents;

/// <summary>
/// Unit tests for <see cref="UpdateConsentCommandHandler"/>.
/// </summary>
public class UpdateConsentCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the consent does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenConsentMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new UpdateConsentCommandHandler(context);

        var command = new UpdateConsentCommand
        {
            Id = Guid.NewGuid(),
            Type = "terms"
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns a failure response when GrantedAt is invalid.
    /// </summary>
    [Fact]
    public async Task Handle_WhenGrantedAtInvalid_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var consent = CreateConsent(context);
        var handler = new UpdateConsentCommandHandler(context);

        var result = await handler.Handle(new UpdateConsentCommand
        {
            Id = consent.Id,
            GrantedAt = "2026-01-01"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("GrantedAt must be in yyyy-MM-dd'T'HH:mm:ss'Z' format.", result.Message);
    }

    /// <summary>
    /// Ensures the handler updates the consent when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_UpdatesConsent_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var consent = CreateConsent(context);
        var handler = new UpdateConsentCommandHandler(context);

        var result = await handler.Handle(new UpdateConsentCommand
        {
            Id = consent.Id,
            IsGranted = false,
            GrantedAt = "2026-02-01T00:00:00Z",
            Version = "v2"
        }, CancellationToken.None);

        Assert.True(result.Success);

        var updated = await context.Consents.SingleAsync(c => c.Id == consent.Id);
        Assert.False(updated.IsGranted);
        Assert.Equal(Instant.FromUtc(2026, 2, 1, 0, 0), updated.GrantedAt);
        Assert.Equal("v2", updated.Version);
    }

    /// <summary>
    /// Creates a valid consent for test scenarios.
    /// </summary>
    /// <param name="context">The test database context.</param>
    /// <returns>The created <see cref="Consent"/> entity.</returns>
    private static Consent CreateConsent(TestApplicationDbContext context)
    {
        var user = CreateUser();
        var consent = new Consent
        {
            UserId = user.Id,
            Type = "terms",
            IsGranted = true,
            GrantedAt = Instant.FromUtc(2026, 1, 1, 0, 0)
        };
        context.Users.Add(user);
        context.Consents.Add(consent);
        context.SaveChanges();
        return consent;
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
