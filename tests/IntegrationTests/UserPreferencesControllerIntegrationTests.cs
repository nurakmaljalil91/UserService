using System.Net;
using System.Net.Http.Json;
using Domain.Common;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the UserPreferencesController covering the full CRUD flow.
/// </summary>
[Collection("Integration")]
public class UserPreferencesControllerIntegrationTests : ApiTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserPreferencesControllerIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory used to create test clients.</param>
    public UserPreferencesControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Performs a full CRUD integration test flow for the UserPreferencesController.
    /// </summary>
    [Fact]
    public async Task UserPreferences_FullFlow_Works()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var userId = await CreateUserAsync(client);

        var listResponse = await client.GetAsync("/api/UserPreferences?page=1&total=10");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var listPayload = await ReadResponseAsync<BaseResponse<PaginatedResponse<UserPreferenceResponse>>>(listResponse);
        Assert.True(listPayload.Success);

        var createResponse = await client.PostAsJsonAsync("/api/UserPreferences", new
        {
            UserId = userId,
            Key = "ui.theme",
            Value = "dark"
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<UserPreferenceResponse>>(createResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        var preferenceId = created.Data!.Id;

        var getResponse = await client.GetAsync($"/api/UserPreferences/{preferenceId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await ReadResponseAsync<BaseResponse<UserPreferenceResponse>>(getResponse);
        Assert.True(fetched.Success);
        Assert.Equal(preferenceId, fetched.Data!.Id);
        Assert.Equal(userId, fetched.Data!.UserId);

        var updateResponse = await client.PatchAsync($"/api/UserPreferences/{preferenceId}", JsonContent.Create(new
        {
            Key = "ui.locale",
            Value = "en-US"
        }));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await ReadResponseAsync<BaseResponse<UserPreferenceResponse>>(updateResponse);
        Assert.True(updated.Success);
        Assert.Equal("ui.locale", updated.Data!.Key);
        Assert.Equal("en-US", updated.Data!.Value);

        var deleteResponse = await client.DeleteAsync($"/api/UserPreferences/{preferenceId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var deletePayload = await ReadResponseAsync<BaseResponse<string>>(deleteResponse);
        Assert.True(deletePayload.Success);

        var getDeletedResponse = await client.GetAsync($"/api/UserPreferences/{preferenceId}");
        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }

    /// <summary>
    /// Creates a user for integration testing and returns the identifier.
    /// </summary>
    /// <param name="client">The HTTP client used to call the API.</param>
    /// <returns>The created user identifier.</returns>
    private static async Task<Guid> CreateUserAsync(HttpClient client)
    {
        var unique = Guid.NewGuid().ToString("N");
        var createUserResponse = await client.PostAsJsonAsync("/api/Users", new
        {
            Username = $"preference-user-{unique}",
            Email = $"preference-{unique}@example.com",
            Password = "Integration123!"
        });
        Assert.Equal(HttpStatusCode.Created, createUserResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<UserResponse>>(createUserResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        return created.Data!.Id;
    }
}
