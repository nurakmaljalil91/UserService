using System.Net;
using System.Net.Http.Json;
using Domain.Common;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the UserProfilesController covering the full CRUD flow.
/// </summary>
[Collection("Integration")]
public class UserProfilesControllerIntegrationTests : ApiTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserProfilesControllerIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory used to create test clients.</param>
    public UserProfilesControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Performs a full CRUD integration test flow for the UserProfilesController.
    /// </summary>
    [Fact]
    public async Task UserProfiles_FullFlow_Works()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var userId = await CreateUserAsync(client);

        var listResponse = await client.GetAsync("/api/UserProfiles?page=1&total=10");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var listPayload = await ReadResponseAsync<BaseResponse<PaginatedResponse<UserProfileResponse>>>(listResponse);
        Assert.True(listPayload.Success);

        var createResponse = await client.PostAsJsonAsync("/api/UserProfiles", new
        {
            UserId = userId,
            DisplayName = "Integration Profile",
            FirstName = "Integration",
            LastName = "User",
            DateOfBirth = "1995-02-03",
            Bio = "Integration bio"
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<UserProfileResponse>>(createResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        var profileId = created.Data!.Id;

        var getResponse = await client.GetAsync($"/api/UserProfiles/{profileId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await ReadResponseAsync<BaseResponse<UserProfileResponse>>(getResponse);
        Assert.True(fetched.Success);
        Assert.Equal(profileId, fetched.Data!.Id);
        Assert.Equal(userId, fetched.Data!.UserId);

        var updateResponse = await client.PatchAsync($"/api/UserProfiles/{profileId}", JsonContent.Create(new
        {
            DisplayName = "Integration Profile Updated",
            Bio = "Integration bio updated",
            DateOfBirth = "1996-03-04"
        }));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await ReadResponseAsync<BaseResponse<UserProfileResponse>>(updateResponse);
        Assert.True(updated.Success);
        Assert.Equal("Integration Profile Updated", updated.Data!.DisplayName);
        Assert.Equal("1996-03-04", updated.Data!.DateOfBirth);

        var deleteResponse = await client.DeleteAsync($"/api/UserProfiles/{profileId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var deletePayload = await ReadResponseAsync<BaseResponse<string>>(deleteResponse);
        Assert.True(deletePayload.Success);

        var getDeletedResponse = await client.GetAsync($"/api/UserProfiles/{profileId}");
        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }

    /// <summary>
    /// Ensures the current user's profile can be retrieved via the me route.
    /// </summary>
    [Fact]
    public async Task GetMyProfile_ReturnsCurrentUserProfile()
    {
        var authenticated = await CreateAuthenticatedClientWithUserAsync();
        using var client = authenticated.Client;
        var userId = authenticated.UserId;

        var createResponse = await client.PostAsJsonAsync("/api/UserProfiles", new
        {
            UserId = userId,
            DisplayName = "Self Profile",
            Bio = "Current user profile"
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var getResponse = await client.GetAsync("/api/UserProfiles/me");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var payload = await ReadResponseAsync<BaseResponse<UserProfileResponse>>(getResponse);
        Assert.True(payload.Success);
        Assert.Equal(userId, payload.Data!.UserId);
        Assert.Equal("Self Profile", payload.Data!.DisplayName);
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
            Username = $"profile-user-{unique}",
            Email = $"profile-{unique}@example.com",
            Password = "Integration123!"
        });
        Assert.Equal(HttpStatusCode.Created, createUserResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<UserResponse>>(createUserResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        return created.Data!.Id;
    }
}
