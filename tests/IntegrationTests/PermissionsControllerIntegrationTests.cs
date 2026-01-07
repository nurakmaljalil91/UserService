using System.Net;
using System.Net.Http.Json;
using Domain.Common;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the PermissionsController covering the full CRUD flow.
/// </summary>
[Collection("Integration")]
public class PermissionsControllerIntegrationTests : ApiTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionsControllerIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory used to create test clients.</param>
    public PermissionsControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Performs a full CRUD integration test flow for the PermissionsController.
    /// </summary>
    [Fact]
    public async Task Permissions_FullFlow_Works()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var listResponse = await client.GetAsync("/api/Permissions?page=1&total=10");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var listPayload = await ReadResponseAsync<BaseResponse<PaginatedResponse<PermissionResponse>>>(listResponse);
        Assert.True(listPayload.Success);

        var unique = Guid.NewGuid().ToString("N");
        var createResponse = await client.PostAsJsonAsync("/api/Permissions", new
        {
            Name = $"permission-{unique}",
            Description = "Integration test permission"
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<PermissionResponse>>(createResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        var permissionId = created.Data!.Id;

        var getResponse = await client.GetAsync($"/api/Permissions/{permissionId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await ReadResponseAsync<BaseResponse<PermissionResponse>>(getResponse);
        Assert.True(fetched.Success);
        Assert.Equal(permissionId, fetched.Data!.Id);

        var updateResponse = await client.PatchAsync($"/api/Permissions/{permissionId}", JsonContent.Create(new
        {
            Name = $"permission-updated-{unique}",
            Description = "Integration test permission updated"
        }));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await ReadResponseAsync<BaseResponse<PermissionResponse>>(updateResponse);
        Assert.True(updated.Success);
        Assert.Equal($"permission-updated-{unique}", updated.Data!.Name);

        var deleteResponse = await client.DeleteAsync($"/api/Permissions/{permissionId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var deletePayload = await ReadResponseAsync<BaseResponse<string>>(deleteResponse);
        Assert.True(deletePayload.Success);

        var getDeletedResponse = await client.GetAsync($"/api/Permissions/{permissionId}");
        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }
}
