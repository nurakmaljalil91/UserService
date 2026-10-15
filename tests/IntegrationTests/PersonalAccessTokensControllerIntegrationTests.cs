#nullable enable
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Authentications.Models;
using Application.PersonalAccessTokens.Models;
using Domain.Common;

namespace IntegrationTests;

/// <summary>
/// End-to-end tests for personal access tokens: create, exchange, scoped use, credential lockout and revocation.
/// </summary>
[Collection("Integration")]
public class PersonalAccessTokensControllerIntegrationTests : ApiTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PersonalAccessTokensControllerIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory.</param>
    public PersonalAccessTokensControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Verifies the full token lifecycle.
    /// </summary>
    [Fact]
    public async Task PersonalAccessToken_FullLifecycle_Works()
    {
        using var interactive = await CreateAuthenticatedClientAsync();

        var scopes = await ReadResponseAsync<BaseResponse<List<string>>>(
            await interactive.GetAsync("/api/PersonalAccessTokens/scopes"));
        Assert.Contains("users:read", scopes.Data!);

        var createResponse = await interactive.PostAsJsonAsync("/api/PersonalAccessTokens", new
        {
            Name = "Integration",
            Scopes = new[] { "users:read" },
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(30)
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = (await ReadResponseAsync<BaseResponse<CreatedPersonalAccessTokenDto>>(createResponse)).Data!;
        Assert.StartsWith("cxs_pat_", created.Token);

        var listed = await ReadResponseAsync<BaseResponse<List<PersonalAccessTokenDto>>>(
            await interactive.GetAsync("/api/PersonalAccessTokens"));
        var listedToken = Assert.Single(listed.Data!);
        Assert.Equal(created.Id, listedToken.Id);
        Assert.DoesNotContain(created.Token, await (await interactive.GetAsync("/api/PersonalAccessTokens")).Content.ReadAsStringAsync());

        using var scoped = CreateClient();
        scoped.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await ExchangeAsync(created.Token));

        Assert.Equal(HttpStatusCode.OK, (await scoped.GetAsync("/api/Users/me")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await scoped.PostAsJsonAsync("/api/UserPreferences", new { Key = "k", Value = "v" })).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await scoped.GetAsync("/api/PersonalAccessTokens")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await scoped.PostAsJsonAsync("/api/PersonalAccessTokens", new { Name = "x", Scopes = new[] { "users:read" } })).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await scoped.GetAsync("/api/Sessions/me")).StatusCode);

        Assert.Equal(HttpStatusCode.OK, (await interactive.DeleteAsync($"/api/PersonalAccessTokens/{created.Id}")).StatusCode);

        using var anonymous = CreateClient();
        var exchangeAfterRevoke = await anonymous.PostAsJsonAsync("/api/Authentications/token-exchange", new { Token = created.Token });
        Assert.Equal(HttpStatusCode.Unauthorized, exchangeAfterRevoke.StatusCode);
    }

    /// <summary>
    /// Verifies that a token scoped to another service is denied on this service.
    /// </summary>
    [Fact]
    public async Task PersonalAccessToken_ForOtherService_IsForbidden()
    {
        using var interactive = await CreateAuthenticatedClientAsync();
        var created = (await ReadResponseAsync<BaseResponse<CreatedPersonalAccessTokenDto>>(
            await interactive.PostAsJsonAsync("/api/PersonalAccessTokens", new { Name = "Planner only", Scopes = new[] { "planner:write" } }))).Data!;

        using var scoped = CreateClient();
        scoped.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await ExchangeAsync(created.Token));

        Assert.Equal(HttpStatusCode.Forbidden, (await scoped.GetAsync("/api/Users/me")).StatusCode);
    }

    /// <summary>
    /// Verifies that revoking another user's token is not found.
    /// </summary>
    [Fact]
    public async Task RevokingAnotherUsersToken_IsNotFound()
    {
        using var owner = await CreateAuthenticatedClientAsync();
        using var other = await CreateAuthenticatedClientAsync();
        var raw = await owner.PostAsJsonAsync("/api/PersonalAccessTokens", new { Name = "Mine", Scopes = new[] { "users:read" } });
        var created = (await ReadResponseAsync<BaseResponse<CreatedPersonalAccessTokenDto>>(raw)).Data!;

        Assert.Equal(HttpStatusCode.NotFound, (await other.DeleteAsync($"/api/PersonalAccessTokens/{created.Id}")).StatusCode);
        Assert.False(string.IsNullOrEmpty(await ExchangeAsync(created.Token)));
    }

    private async Task<string> ExchangeAsync(string secret)
    {
        using var anonymous = CreateClient();
        var response = await anonymous.PostAsJsonAsync("/api/Authentications/token-exchange", new { Token = secret });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await ReadResponseAsync<BaseResponse<TokenExchangeResponse>>(response)).Data!.Token;
    }
}
