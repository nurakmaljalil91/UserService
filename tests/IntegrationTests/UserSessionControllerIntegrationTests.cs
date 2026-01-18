using System.Net;
using System.Net.Http.Json;
using Domain.Common;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the UserSessionController.
/// </summary>
[Collection("Integration")]
public class UserSessionControllerIntegrationTests : ApiTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserSessionControllerIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory used to create test clients.</param>
    public UserSessionControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Ensures the user session endpoint returns the current user's details.
    /// </summary>
    [Fact]
    public async Task GetUserSession_ReturnsCurrentUserSession()
    {
        var authenticated = await CreateAuthenticatedClientWithUserAsync();
        using var client = authenticated.Client;
        var userId = authenticated.UserId;

        var createProfileResponse = await client.PostAsJsonAsync("/api/UserProfiles", new
        {
            UserId = userId,
            DisplayName = "Session User"
        });
        Assert.Equal(HttpStatusCode.Created, createProfileResponse.StatusCode);

        var createPreferenceResponse = await client.PostAsJsonAsync("/api/UserPreferences", new
        {
            UserId = userId,
            Key = "theme",
            Value = "dark"
        });
        Assert.Equal(HttpStatusCode.Created, createPreferenceResponse.StatusCode);

        var sessionResponse = await client.GetAsync("/api/UserSession");
        Assert.Equal(HttpStatusCode.OK, sessionResponse.StatusCode);

        var payload = await ReadResponseAsync<BaseResponse<UserSessionResponse>>(sessionResponse);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Data);
        Assert.Equal(userId, payload.Data!.User!.Id);
        Assert.Equal("Session User", payload.Data!.Profile!.DisplayName);
        Assert.Single(payload.Data!.Preferences!);
    }

    /// <summary>
    /// Ensures the user session endpoint returns not found when the profile is missing.
    /// </summary>
    [Fact]
    public async Task GetUserSession_WhenProfileMissing_ReturnsNotFound()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var sessionResponse = await client.GetAsync("/api/UserSession");

        Assert.Equal(HttpStatusCode.NotFound, sessionResponse.StatusCode);
    }
}
