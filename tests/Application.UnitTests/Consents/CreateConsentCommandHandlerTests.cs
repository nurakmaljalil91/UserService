#nullable enable
using Application.Consents.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.UnitTests.Consents;

/// <summary>
/// Unit tests for <see cref="CreateConsentCommandHandler"/>.
/// </summary>
public class CreateConsentCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler creates a consent when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_CreatesConsent_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateConsentCommandHandler(context);

        var result = await handler.Handle(new CreateConsentCommand
        {
            UserId = user.Id,
            Type = " terms ",
            IsGranted = true,
            GrantedAt = "2026-01-01T00:00:00Z",
            Version = " v1 "
        }, CancellationToken.None);

        Assert.True(result.Success);

        var consent = await context.Consents.SingleAsync();
        Assert.Equal(user.Id, consent.UserId);
        Assert.Equal("terms", consent.Type);
        Assert.True(consent.IsGranted);
        Assert.Equal(Instant.FromUtc(2026, 1, 1, 0, 0), consent.GrantedAt);
        Assert.Equal("v1", consent.Version);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserMissing_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new CreateConsentCommandHandler(context);

        var result = await handler.Handle(new CreateConsentCommand
        {
            UserId = Guid.NewGuid(),
            Type = "terms",
            IsGranted = true,
            GrantedAt = "2026-01-01T00:00:00Z"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User does not exist.", result.Message);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when GrantedAt is invalid.
    /// </summary>
    [Fact]
    public async Task Handle_WhenGrantedAtInvalid_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateConsentCommandHandler(context);

        var result = await handler.Handle(new CreateConsentCommand
        {
            UserId = user.Id,
            Type = "terms",
            IsGranted = true,
            GrantedAt = "2026-01-01"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("GrantedAt must be in yyyy-MM-dd'T'HH:mm:ss'Z' format.", result.Message);
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
