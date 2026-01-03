#nullable enable
using System.Net;
using System.Net.Http.Json;
using Domain.Common;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the UsersController covering the full CRUD flow.
/// </summary>
[Collection("Integration")]
public class UsersControllerIntegrationTests : ApiTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UsersControllerIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory used to create test clients.</param>
    public UsersControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Performs a full CRUD integration test flow for the UsersController.
    /// </summary>
    [Fact]
    public async Task Users_FullFlow_Works()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var listResponse = await client.GetAsync("/api/Users?page=1&total=10");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var listPayload = await ReadResponseAsync<BaseResponse<PaginatedResponse<UserResponse>>>(listResponse);
        Assert.True(listPayload.Success);

        var createResponse = await client.PostAsJsonAsync("/api/Users", new
        {
            Username = $"user-{Guid.NewGuid():N}",
            Email = $"user-{Guid.NewGuid():N}@example.com",
            PhoneNumber = "+12025550123",
            Password = "User123!"
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<UserResponse>>(createResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        var userId = created.Data!.Id;

        var getResponse = await client.GetAsync($"/api/Users/{userId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await ReadResponseAsync<BaseResponse<UserResponse>>(getResponse);
        Assert.True(fetched.Success);
        Assert.Equal(userId, fetched.Data!.Id);

        var updateResponse = await client.PatchAsync($"/api/Users/{userId}", JsonContent.Create(new
        {
            Username = "updated-user",
            Email = $"updated-{Guid.NewGuid():N}@example.com",
            PhoneNumber = "+12025550124",
            IsLocked = false
        }));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await ReadResponseAsync<BaseResponse<UserResponse>>(updateResponse);
        Assert.True(updated.Success);
        Assert.Equal("updated-user", updated.Data!.Username);

        var deleteResponse = await client.DeleteAsync($"/api/Users/{userId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var deletePayload = await ReadResponseAsync<BaseResponse<string>>(deleteResponse);
        Assert.True(deletePayload.Success);

        var getDeletedResponse = await client.GetAsync($"/api/Users/{userId}");
        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }
}
