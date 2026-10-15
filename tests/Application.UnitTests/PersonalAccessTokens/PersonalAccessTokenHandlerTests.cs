#nullable enable
using Application.Common.Exceptions;
using Application.PersonalAccessTokens;
using Application.PersonalAccessTokens.Commands;
using Application.PersonalAccessTokens.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Domain.ValueObjects;
using NodaTime;

namespace Application.UnitTests.PersonalAccessTokens;

/// <summary>
/// Unit tests for the personal access token create, list, revoke and scope handlers.
/// </summary>
public class PersonalAccessTokenHandlerTests
{
    private static readonly TestClockService Clock = new();

    /// <summary>
    /// Verifies that creating a token returns its secret once, stores only its hash, and expands write scopes.
    /// </summary>
    [Fact]
    public async Task Create_ReturnsSecretOnceAndStoresHash()
    {
        await using var context = TestDbContextFactory.Create();
        var user = new TestUser { UserId = Guid.NewGuid() };
        var expiresAt = Clock.Now.ToDateTimeOffset().AddDays(90);

        var result = await CreateHandler(context, user).Handle(
            new CreatePersonalAccessTokenCommand { Name = " Claude ", Scopes = ["planner:write"], ExpiresAt = expiresAt },
            CancellationToken.None);

        var dto = result.Data!;
        Assert.StartsWith(PersonalAccessTokenSecret.Prefix, dto.Token);
        Assert.Equal("Claude", dto.Name);
        Assert.Equal(dto.Token[..PersonalAccessTokenSecret.DisplayPrefixLength], dto.DisplayPrefix);
        Assert.Equal(new[] { "planner:read", "planner:write" }, dto.Scopes);
        Assert.True(dto.IsActive);

        var stored = Assert.Single(context.PersonalAccessTokens);
        Assert.Equal(user.UserId, stored.UserId);
        Assert.Equal(new TestRefreshTokenHasher().Hash(dto.Token), stored.TokenHash);
        Assert.Equal(Instant.FromDateTimeOffset(expiresAt), stored.ExpiresAt);
    }

    /// <summary>
    /// Verifies that secrets are unique.
    /// </summary>
    [Fact]
    public void Secret_IsUniqueAndUrlSafe()
    {
        var secrets = Enumerable.Range(0, 50).Select(_ => PersonalAccessTokenSecret.Create()).ToList();

        Assert.Equal(50, secrets.Distinct().Count());
        Assert.All(secrets, s => Assert.Matches("^cxs_pat_[A-Za-z0-9_-]{43}$", s));
    }

    /// <summary>
    /// Verifies validation of name, scopes and expiry.
    /// </summary>
    [Fact]
    public void Validator_RejectsMissingNameUnsupportedScopeAndPastExpiry()
    {
        var validator = new CreatePersonalAccessTokenCommandValidator(Clock);

        Assert.True(validator.Validate(new CreatePersonalAccessTokenCommand { Name = "ok", Scopes = ["planner:read"] }).IsValid);
        Assert.False(validator.Validate(new CreatePersonalAccessTokenCommand { Name = "", Scopes = ["planner:read"] }).IsValid);
        Assert.False(validator.Validate(new CreatePersonalAccessTokenCommand { Name = "ok", Scopes = [] }).IsValid);
        Assert.False(validator.Validate(new CreatePersonalAccessTokenCommand { Name = "ok", Scopes = ["planner:admin"] }).IsValid);
        Assert.False(validator.Validate(new CreatePersonalAccessTokenCommand
        {
            Name = "ok",
            Scopes = ["planner:read"],
            ExpiresAt = Clock.Now.ToDateTimeOffset()
        }).IsValid);
    }

    /// <summary>
    /// Verifies that listing returns only the caller's tokens, newest first, without secrets.
    /// </summary>
    [Fact]
    public async Task List_ReturnsOnlyOwnTokens()
    {
        await using var context = TestDbContextFactory.Create();
        var me = Guid.NewGuid();
        context.PersonalAccessTokens.AddRange(
            Token(me, "older", Clock.Now - Duration.FromDays(2)),
            Token(me, "newer", Clock.Now - Duration.FromDays(1)),
            Token(Guid.NewGuid(), "someone else", Clock.Now));
        await context.SaveChangesAsync();

        var result = await new GetMyPersonalAccessTokensQueryHandler(context, new TestUser { UserId = me }, Clock)
            .Handle(new GetMyPersonalAccessTokensQuery(), CancellationToken.None);

        Assert.Equal(new[] { "newer", "older" }, result.Data!.Select(t => t.Name));
    }

    /// <summary>
    /// Verifies that the owner can revoke a token and the token becomes inactive.
    /// </summary>
    [Fact]
    public async Task Revoke_OwnToken_MarksRevoked()
    {
        await using var context = TestDbContextFactory.Create();
        var me = Guid.NewGuid();
        var token = Token(me, "mine", Clock.Now);
        context.PersonalAccessTokens.Add(token);
        await context.SaveChangesAsync();

        var result = await new RevokePersonalAccessTokenCommandHandler(context, new TestUser { UserId = me }, Clock)
            .Handle(new RevokePersonalAccessTokenCommand { Id = token.Id }, CancellationToken.None);

        Assert.False(result.Data!.IsActive);
        Assert.Equal(Clock.Now, token.RevokedAt);
    }

    /// <summary>
    /// Verifies that revoking another user's token is reported as not found and leaves it active.
    /// </summary>
    [Fact]
    public async Task Revoke_OtherUsersToken_ThrowsNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var token = Token(Guid.NewGuid(), "theirs", Clock.Now);
        context.PersonalAccessTokens.Add(token);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            new RevokePersonalAccessTokenCommandHandler(context, new TestUser { UserId = Guid.NewGuid() }, Clock)
                .Handle(new RevokePersonalAccessTokenCommand { Id = token.Id }, CancellationToken.None));
        Assert.Null(token.RevokedAt);
    }

    /// <summary>
    /// Verifies that the supported scope list is exposed.
    /// </summary>
    [Fact]
    public async Task GetScopes_ReturnsAllSupportedScopes()
    {
        var result = await new GetTokenScopesQueryHandler().Handle(new GetTokenScopesQuery(), CancellationToken.None);

        Assert.Equal(TokenScope.All, result.Data);
    }

    private static CreatePersonalAccessTokenCommandHandler CreateHandler(TestApplicationDbContext context, TestUser user)
        => new(context, user, Clock, new TestRefreshTokenHasher());

    private static PersonalAccessToken Token(Guid userId, string name, Instant created) => new()
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        Name = name,
        DisplayPrefix = "cxs_pat_abcd",
        TokenHash = Guid.NewGuid().ToString(),
        Scopes = ["planner:read"],
        CreatedDate = created
    };
}
