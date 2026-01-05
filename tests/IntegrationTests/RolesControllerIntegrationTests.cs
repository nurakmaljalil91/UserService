using System.Net;
using System.Net.Http.Json;
using Domain.Common;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the RolesController covering the full CRUD flow.
/// </summary>
[Collection("Integration")]
public class RolesControllerIntegrationTests : ApiTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RolesControllerIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory used to create test clients.</param>
    public RolesControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Performs a full CRUD integration test flow for the RolesController.
    /// </summary>
    [Fact]
    public async Task Roles_FullFlow_Works()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var listResponse = await client.GetAsync("/api/Roles?page=1&total=10");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var listPayload = await ReadResponseAsync<BaseResponse<PaginatedResponse<RoleResponse>>>(listResponse);
        Assert.True(listPayload.Success);

        var unique = Guid.NewGuid().ToString("N");
        var createResponse = await client.PostAsJsonAsync("/api/Roles", new
        {
            Name = $"role-{unique}",
            Description = "Integration test role"
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<RoleResponse>>(createResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        var roleId = created.Data!.Id;

        var getResponse = await client.GetAsync($"/api/Roles/{roleId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await ReadResponseAsync<BaseResponse<RoleResponse>>(getResponse);
        Assert.True(fetched.Success);
        Assert.Equal(roleId, fetched.Data!.Id);

        var updateResponse = await client.PatchAsync($"/api/Roles/{roleId}", JsonContent.Create(new
        {
            Name = $"role-updated-{unique}",
            Description = "Integration test role updated"
        }));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await ReadResponseAsync<BaseResponse<RoleResponse>>(updateResponse);
        Assert.True(updated.Success);
        Assert.Equal($"role-updated-{unique}", updated.Data!.Name);

        var deleteResponse = await client.DeleteAsync($"/api/Roles/{roleId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var deletePayload = await ReadResponseAsync<BaseResponse<string>>(deleteResponse);
        Assert.True(deletePayload.Success);

        var getDeletedResponse = await client.GetAsync($"/api/Roles/{roleId}");
        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }
}
