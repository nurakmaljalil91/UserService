using System.Net;
using System.Net.Http.Json;
using Domain.Common;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the AddressesController covering the full CRUD flow.
/// </summary>
[Collection("Integration")]
public class AddressesControllerIntegrationTests : ApiTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddressesControllerIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory used to create test clients.</param>
    public AddressesControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Performs a full CRUD integration test flow for the AddressesController.
    /// </summary>
    [Fact]
    public async Task Addresses_FullFlow_Works()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var userId = await CreateUserAsync(client);

        var listResponse = await client.GetAsync("/api/Addresses?page=1&total=10");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var listPayload = await ReadResponseAsync<BaseResponse<PaginatedResponse<AddressResponse>>>(listResponse);
        Assert.True(listPayload.Success);

        var createResponse = await client.PostAsJsonAsync("/api/Addresses", new
        {
            UserId = userId,
            Label = "Home",
            Type = "home",
            Line1 = "123 Main St",
            Line2 = "Unit 5",
            City = "Metro",
            State = "State",
            PostalCode = "12345",
            Country = "Country",
            IsDefault = true
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<AddressResponse>>(createResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        var addressId = created.Data!.Id;

        var getResponse = await client.GetAsync($"/api/Addresses/{addressId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await ReadResponseAsync<BaseResponse<AddressResponse>>(getResponse);
        Assert.True(fetched.Success);
        Assert.Equal(addressId, fetched.Data!.Id);
        Assert.Equal(userId, fetched.Data!.UserId);

        var updateResponse = await client.PatchAsync($"/api/Addresses/{addressId}", JsonContent.Create(new
        {
            Label = "Home Updated",
            City = "Metro City",
            IsDefault = false
        }));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await ReadResponseAsync<BaseResponse<AddressResponse>>(updateResponse);
        Assert.True(updated.Success);
        Assert.Equal("Home Updated", updated.Data!.Label);
        Assert.False(updated.Data!.IsDefault);

        var deleteResponse = await client.DeleteAsync($"/api/Addresses/{addressId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var deletePayload = await ReadResponseAsync<BaseResponse<string>>(deleteResponse);
        Assert.True(deletePayload.Success);

        var getDeletedResponse = await client.GetAsync($"/api/Addresses/{addressId}");
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
            Username = $"address-user-{unique}",
            Email = $"address-{unique}@example.com",
            Password = "Integration123!"
        });
        Assert.Equal(HttpStatusCode.Created, createUserResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<UserResponse>>(createUserResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        return created.Data!.Id;
    }
}
