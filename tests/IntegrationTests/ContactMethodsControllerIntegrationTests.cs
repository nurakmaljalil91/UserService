using System.Net;
using System.Net.Http.Json;
using Domain.Common;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the ContactMethodsController covering the full CRUD flow.
/// </summary>
[Collection("Integration")]
public class ContactMethodsControllerIntegrationTests : ApiTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContactMethodsControllerIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory used to create test clients.</param>
    public ContactMethodsControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Performs a full CRUD integration test flow for the ContactMethodsController.
    /// </summary>
    [Fact]
    public async Task ContactMethods_FullFlow_Works()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var userId = await CreateUserAsync(client);

        var listResponse = await client.GetAsync("/api/ContactMethods?page=1&total=10");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var listPayload = await ReadResponseAsync<BaseResponse<PaginatedResponse<ContactMethodResponse>>>(listResponse);
        Assert.True(listPayload.Success);

        var createResponse = await client.PostAsJsonAsync("/api/ContactMethods", new
        {
            UserId = userId,
            Type = "email",
            Value = "user@example.com",
            IsVerified = true,
            IsPrimary = true
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<ContactMethodResponse>>(createResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        var contactMethodId = created.Data!.Id;

        var getResponse = await client.GetAsync($"/api/ContactMethods/{contactMethodId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await ReadResponseAsync<BaseResponse<ContactMethodResponse>>(getResponse);
        Assert.True(fetched.Success);
        Assert.Equal(contactMethodId, fetched.Data!.Id);
        Assert.Equal(userId, fetched.Data!.UserId);

        var updateResponse = await client.PatchAsync($"/api/ContactMethods/{contactMethodId}", JsonContent.Create(new
        {
            Type = "phone",
            Value = "+1234567890",
            IsVerified = false,
            IsPrimary = false
        }));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await ReadResponseAsync<BaseResponse<ContactMethodResponse>>(updateResponse);
        Assert.True(updated.Success);
        Assert.Equal("phone", updated.Data!.Type);
        Assert.Equal("+1234567890", updated.Data!.Value);
        Assert.False(updated.Data!.IsVerified);

        var deleteResponse = await client.DeleteAsync($"/api/ContactMethods/{contactMethodId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var deletePayload = await ReadResponseAsync<BaseResponse<string>>(deleteResponse);
        Assert.True(deletePayload.Success);

        var getDeletedResponse = await client.GetAsync($"/api/ContactMethods/{contactMethodId}");
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
            Username = $"contact-user-{unique}",
            Email = $"contact-{unique}@example.com",
            Password = "Integration123!"
        });
        Assert.Equal(HttpStatusCode.Created, createUserResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<UserResponse>>(createUserResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        return created.Data!.Id;
    }
}
