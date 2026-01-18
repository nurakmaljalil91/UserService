using System.Net;
using System.Net.Http.Json;
using Domain.Common;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the ConsentsController covering the full CRUD flow.
/// </summary>
[Collection("Integration")]
public class ConsentsControllerIntegrationTests : ApiTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConsentsControllerIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory used to create test clients.</param>
    public ConsentsControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Performs a full CRUD integration test flow for the ConsentsController.
    /// </summary>
    [Fact]
    public async Task Consents_FullFlow_Works()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var userId = await CreateUserAsync(client);

        var listResponse = await client.GetAsync("/api/Consents?page=1&total=10");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var listPayload = await ReadResponseAsync<BaseResponse<PaginatedResponse<ConsentResponse>>>(listResponse);
        Assert.True(listPayload.Success);

        var createResponse = await client.PostAsJsonAsync("/api/Consents", new
        {
            UserId = userId,
            Type = "terms",
            IsGranted = true,
            GrantedAt = "2026-01-01T00:00:00Z",
            Version = "v1",
            Source = "web"
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<ConsentResponse>>(createResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        var consentId = created.Data!.Id;

        var getResponse = await client.GetAsync($"/api/Consents/{consentId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await ReadResponseAsync<BaseResponse<ConsentResponse>>(getResponse);
        Assert.True(fetched.Success);
        Assert.Equal(consentId, fetched.Data!.Id);
        Assert.Equal(userId, fetched.Data!.UserId);

        var updateResponse = await client.PatchAsync($"/api/Consents/{consentId}", JsonContent.Create(new
        {
            IsGranted = false,
            GrantedAt = "2026-02-01T00:00:00Z",
            Version = "v2"
        }));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await ReadResponseAsync<BaseResponse<ConsentResponse>>(updateResponse);
        Assert.True(updated.Success);
        Assert.False(updated.Data!.IsGranted);
        Assert.Equal("v2", updated.Data!.Version);

        var deleteResponse = await client.DeleteAsync($"/api/Consents/{consentId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var deletePayload = await ReadResponseAsync<BaseResponse<string>>(deleteResponse);
        Assert.True(deletePayload.Success);

        var getDeletedResponse = await client.GetAsync($"/api/Consents/{consentId}");
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
            Username = $"consent-user-{unique}",
            Email = $"consent-{unique}@example.com",
            Password = "Integration123!"
        });
        Assert.Equal(HttpStatusCode.Created, createUserResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<UserResponse>>(createUserResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        return created.Data!.Id;
    }
}
