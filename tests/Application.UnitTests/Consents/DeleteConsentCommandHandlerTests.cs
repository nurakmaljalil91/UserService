#nullable enable
using Application.Common.Exceptions;
using Application.Consents.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.UnitTests.Consents;

/// <summary>
/// Unit tests for <see cref="DeleteConsentCommandHandler"/>.
/// </summary>
public class DeleteConsentCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the consent does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenConsentMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new DeleteConsentCommandHandler(context);

        var command = new DeleteConsentCommand
        {
            Id = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler deletes the consent when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_DeletesConsent_WhenExists()
    {
        await using var context = TestDbContextFactory.Create();
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
        await context.SaveChangesAsync();

        var handler = new DeleteConsentCommandHandler(context);

        var result = await handler.Handle(new DeleteConsentCommand
        {
            Id = consent.Id
        }, CancellationToken.None);

        Assert.True(result.Success);
        var remaining = await context.Consents.CountAsync();
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
