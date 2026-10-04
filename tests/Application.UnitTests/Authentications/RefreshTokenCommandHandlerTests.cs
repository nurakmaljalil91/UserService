#nullable enable
using System.IdentityModel.Tokens.Jwt;
using Application.Authentications.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using NodaTime;

namespace Application.UnitTests.Authentications;

/// <summary>
/// Contains unit tests for <see cref="RefreshTokenCommandHandler"/>.
/// </summary>
public class RefreshTokenCommandHandlerTests
{
    private const string RefreshToken = "refresh-token-value";

    /// <summary>Ensures a refreshed access token carries the user's current time zone as the zoneinfo claim.</summary>
    [Fact]
    public async Task Handle_IncludesZoneInfoClaim_WhenTimeZonePreferenceExists()
    {
        await using var context = TestDbContextFactory.Create();
        var user = await SeedUserWithSessionAsync(context);
        context.UserPreferences.Add(new UserPreference { UserId = user.Id, Key = "timezone", Value = "Asia/Kuala_Lumpur" });
        await context.SaveChangesAsync();

        var result = await CreateHandler(context).Handle(
            new RefreshTokenCommand { RefreshToken = RefreshToken },
            CancellationToken.None);

        Assert.True(result.Success);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Data!.Token);
        Assert.Equal("Asia/Kuala_Lumpur", token.Claims.Single(c => c.Type == "zoneinfo").Value);
    }

    /// <summary>Ensures a refreshed access token has no zoneinfo claim when no time zone is saved.</summary>
    [Fact]
    public async Task Handle_OmitsZoneInfoClaim_WhenNoTimeZonePreference()
    {
        await using var context = TestDbContextFactory.Create();
        await SeedUserWithSessionAsync(context);

        var result = await CreateHandler(context).Handle(
            new RefreshTokenCommand { RefreshToken = RefreshToken },
            CancellationToken.None);

        Assert.True(result.Success);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Data!.Token);
        Assert.DoesNotContain(token.Claims, c => c.Type == "zoneinfo");
    }

    private static async Task<User> SeedUserWithSessionAsync(TestApplicationDbContext context)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "refreshuser",
            NormalizedUsername = "REFRESHUSER",
            Email = "refreshuser@example.com",
            NormalizedEmail = "REFRESHUSER@EXAMPLE.COM",
            IsDeleted = false,
            IsLocked = false
        };
        context.Users.Add(user);
        context.Sessions.Add(new Session
        {
            UserId = user.Id,
            RefreshToken = new TestRefreshTokenHasher().Hash(RefreshToken),
            ExpiresAt = Instant.FromUtc(2025, 2, 1, 0, 0),
            IsRevoked = false
        });
        await context.SaveChangesAsync();
        return user;
    }

    private static RefreshTokenCommandHandler CreateHandler(TestApplicationDbContext context)
        => new(
            context,
            new TestDateTime(),
            new TestClockService(),
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Issuer"] = "Application.UnitTests",
                    ["Jwt:Audience"] = "Application.UnitTests",
                    ["Jwt:Key"] = "application-unit-tests-super-secret-key-1234567890",
                    ["Jwt:ExpiryMinutes"] = "60",
                    ["Jwt:RefreshTokenExpiryDays"] = "30"
                })
                .Build(),
            new TestRefreshTokenHasher());
}
