using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Application.Authentications.Models;
using Domain.Common;
using WebAPI.Controllers;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the <see cref="AuthenticationsController"/> endpoints.
/// </summary>
[Collection("Integration")]
public class AuthenticationsControllerIntegrationTests : ApiTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationsControllerIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory used to create the test server and client.</param>
    public AuthenticationsControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Tests that the login endpoint returns a valid authentication token for a newly registered user.
    /// </summary>
    [Fact]
    public async Task Login_ReturnsToken()
    {
        using var client = CreateClient();

        var unique = Guid.NewGuid().ToString("N");
        var username = $"user-{unique}";
        var email = $"user-{unique}@example.com";

        var registerResponse = await client.PostAsJsonAsync("/api/Authentications/register", new
        {
            Username = username,
            Email = email,
            Password = "pass123!"
        });

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var response = await client.PostAsJsonAsync("/api/Authentications/login", new
        {
            Username = username,
            Email = email,
            Password = "pass123!"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await ReadResponseAsync<BaseResponse<LoginResponse>>(response);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Data);
        Assert.False(string.IsNullOrWhiteSpace(payload.Data!.Token));
        Assert.True(payload.Data.ShowFirstLoginWelcome);
        Assert.True(payload.Data!.ExpiresAt > DateTime.UtcNow.AddMinutes(-1));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", payload.Data.Token);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/UserSession")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/UserProfiles/me")).StatusCode);

        using var secondClient = CreateClient();
        var laterLogin = await secondClient.PostAsJsonAsync("/api/Authentications/login", new
        {
            Username = username,
            Password = "pass123!"
        });
        var laterPayload = await ReadResponseAsync<BaseResponse<LoginResponse>>(laterLogin);
        Assert.False(laterPayload.Data!.ShowFirstLoginWelcome);

        var skip = await client.PutAsJsonAsync("/api/onboarding/me", new { Outcome = "Skipped" });
        Assert.Equal(HttpStatusCode.OK, skip.StatusCode);
        Assert.Equal(HttpStatusCode.OK,
            (await client.PutAsJsonAsync("/api/onboarding/me", new { Outcome = "Skipped" })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest,
            (await client.PutAsJsonAsync("/api/onboarding/me", new { Outcome = "Completed" })).StatusCode);
    }

    /// <summary>Ensures the onboarding endpoint requires an authenticated account.</summary>
    [Fact]
    public async Task Onboarding_RejectsAnonymousRequest()
    {
        using var client = CreateClient();
        var response = await client.PutAsJsonAsync("/api/onboarding/me", new { Outcome = "Skipped" });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>Ensures profile setup can be saved and completed without another welcome.</summary>
    [Fact]
    public async Task Onboarding_RecordsCompletedAfterProfileUpdate()
    {
        using var client = CreateClient();
        var unique = Guid.NewGuid().ToString("N");
        var username = $"profile-{unique}";
        var credentials = new { Username = username, Email = $"{username}@example.com", Password = "pass123!" };
        Assert.Equal(HttpStatusCode.OK,
            (await client.PostAsJsonAsync("/api/Authentications/register", credentials)).StatusCode);

        var login = await client.PostAsJsonAsync("/api/Authentications/login", credentials);
        var loginPayload = await ReadResponseAsync<BaseResponse<LoginResponse>>(login);
        Assert.True(loginPayload.Data!.ShowFirstLoginWelcome);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginPayload.Data.Token);

        var profileResponse = await client.GetAsync("/api/UserProfiles/me");
        var profile = await ReadResponseAsync<BaseResponse<UserProfileResponse>>(profileResponse);
        Assert.NotNull(profile.Data);
        var update = await client.PatchAsJsonAsync($"/api/UserProfiles/{profile.Data!.Id}",
            new { DisplayName = "New Member" });
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);
        Assert.Equal(HttpStatusCode.OK,
            (await client.PutAsJsonAsync("/api/onboarding/me", new { Outcome = "Completed" })).StatusCode);

        using var secondClient = CreateClient();
        var laterLogin = await secondClient.PostAsJsonAsync("/api/Authentications/login", credentials);
        var laterPayload = await ReadResponseAsync<BaseResponse<LoginResponse>>(laterLogin);
        Assert.False(laterPayload.Data!.ShowFirstLoginWelcome);
    }

    /// <summary>Ensures concurrent logins cannot both claim the welcome.</summary>
    [Fact]
    public async Task Login_ConcurrentRequestsShowWelcomeAtMostOnce()
    {
        using var firstClient = CreateClient();
        using var secondClient = CreateClient();
        var unique = Guid.NewGuid().ToString("N");
        var credentials = new
        {
            Username = $"race-{unique}",
            Email = $"race-{unique}@example.com",
            Password = "pass123!"
        };

        Assert.Equal(HttpStatusCode.OK,
            (await firstClient.PostAsJsonAsync("/api/Authentications/register", credentials)).StatusCode);

        var responses = await Task.WhenAll(
            firstClient.PostAsJsonAsync("/api/Authentications/login", credentials),
            secondClient.PostAsJsonAsync("/api/Authentications/login", credentials));
        Assert.All(responses, response => Assert.Equal(HttpStatusCode.OK, response.StatusCode));
        var first = await ReadResponseAsync<BaseResponse<LoginResponse>>(responses[0]);
        var second = await ReadResponseAsync<BaseResponse<LoginResponse>>(responses[1]);
        Assert.Equal(1, new[] { first.Data!.ShowFirstLoginWelcome, second.Data!.ShowFirstLoginWelcome }
            .Count(show => show));
    }
}
