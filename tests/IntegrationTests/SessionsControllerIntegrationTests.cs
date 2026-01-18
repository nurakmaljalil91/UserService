using System.Net;
using System.Net.Http.Json;
using Domain.Common;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the SessionsController covering the full CRUD flow.
/// </summary>
[Collection("Integration")]
public class SessionsControllerIntegrationTests : ApiTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SessionsControllerIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory used to create test clients.</param>
    public SessionsControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Performs a full CRUD integration test flow for the SessionsController.
    /// </summary>
    [Fact]
    public async Task Sessions_FullFlow_Works()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var userId = await CreateUserAsync(client);

        var listResponse = await client.GetAsync("/api/Sessions?page=1&total=10");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var listPayload = await ReadResponseAsync<BaseResponse<PaginatedResponse<SessionResponse>>>(listResponse);
        Assert.True(listPayload.Success);

        var createResponse = await client.PostAsJsonAsync("/api/Sessions", new
        {
            UserId = userId,
            RefreshToken = "refresh-token-1",
            ExpiresAt = "2026-01-01T00:00:00Z",
            IpAddress = "127.0.0.1",
            UserAgent = "IntegrationTest",
            DeviceName = "TestDevice"
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<SessionResponse>>(createResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        var sessionId = created.Data!.Id;

        var getResponse = await client.GetAsync($"/api/Sessions/{sessionId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await ReadResponseAsync<BaseResponse<SessionResponse>>(getResponse);
        Assert.True(fetched.Success);
        Assert.Equal(sessionId, fetched.Data!.Id);
        Assert.Equal(userId, fetched.Data!.UserId);

        var updateResponse = await client.PatchAsync($"/api/Sessions/{sessionId}", JsonContent.Create(new
        {
            RefreshToken = "refresh-token-2",
            RevokedAt = "2026-01-02T00:00:00Z",
            IsRevoked = true
        }));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await ReadResponseAsync<BaseResponse<SessionResponse>>(updateResponse);
        Assert.True(updated.Success);
        Assert.Equal("refresh-token-2", updated.Data!.RefreshToken);
        Assert.True(updated.Data!.IsRevoked);

        var deleteResponse = await client.DeleteAsync($"/api/Sessions/{sessionId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var deletePayload = await ReadResponseAsync<BaseResponse<string>>(deleteResponse);
        Assert.True(deletePayload.Success);

        var getDeletedResponse = await client.GetAsync($"/api/Sessions/{sessionId}");
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
            Username = $"session-user-{unique}",
            Email = $"session-{unique}@example.com",
            Password = "Integration123!"
        });
        Assert.Equal(HttpStatusCode.Created, createUserResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<UserResponse>>(createUserResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        return created.Data!.Id;
    }
}
