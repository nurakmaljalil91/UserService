using System.Net;
using System.Net.Http.Json;
using Domain.Common;

namespace IntegrationTests;

/// <summary>
/// Integration tests for the GroupsController covering the full CRUD flow.
/// </summary>
[Collection("Integration")]
public class GroupsControllerIntegrationTests : ApiTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupsControllerIntegrationTests"/> class.
    /// </summary>
    /// <param name="factory">The API factory used to create test clients.</param>
    public GroupsControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    /// <summary>
    /// Performs a full CRUD integration test flow for the GroupsController.
    /// </summary>
    [Fact]
    public async Task Groups_FullFlow_Works()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var listResponse = await client.GetAsync("/api/Groups?page=1&total=10");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var listPayload = await ReadResponseAsync<BaseResponse<PaginatedResponse<GroupResponse>>>(listResponse);
        Assert.True(listPayload.Success);

        var unique = Guid.NewGuid().ToString("N");
        var createResponse = await client.PostAsJsonAsync("/api/Groups", new
        {
            Name = $"group-{unique}",
            Description = "Integration test group"
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<GroupResponse>>(createResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        var groupId = created.Data!.Id;

        var getResponse = await client.GetAsync($"/api/Groups/{groupId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await ReadResponseAsync<BaseResponse<GroupResponse>>(getResponse);
        Assert.True(fetched.Success);
        Assert.Equal(groupId, fetched.Data!.Id);

        var updateResponse = await client.PatchAsync($"/api/Groups/{groupId}", JsonContent.Create(new
        {
            Name = $"group-updated-{unique}",
            Description = "Integration test group updated"
        }));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await ReadResponseAsync<BaseResponse<GroupResponse>>(updateResponse);
        Assert.True(updated.Success);
        Assert.Equal($"group-updated-{unique}", updated.Data!.Name);

        var deleteResponse = await client.DeleteAsync($"/api/Groups/{groupId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var deletePayload = await ReadResponseAsync<BaseResponse<string>>(deleteResponse);
        Assert.True(deletePayload.Success);

        var getDeletedResponse = await client.GetAsync($"/api/Groups/{groupId}");
        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }
}
