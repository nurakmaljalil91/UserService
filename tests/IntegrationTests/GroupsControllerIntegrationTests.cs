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

    /// <summary>
    /// Ensures roles can be assigned to a group.
    /// </summary>
    [Fact]
    public async Task Groups_AssignRole_Works()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var unique = Guid.NewGuid().ToString("N");
        var roleResponse = await client.PostAsJsonAsync("/api/Roles", new
        {
            Name = $"role-{unique}",
            Description = "Integration test role"
        });
        Assert.Equal(HttpStatusCode.Created, roleResponse.StatusCode);

        var createdRole = await ReadResponseAsync<BaseResponse<RoleResponse>>(roleResponse);
        Assert.True(createdRole.Success);
        var roleId = createdRole.Data!.Id;

        var groupResponse = await client.PostAsJsonAsync("/api/Groups", new
        {
            Name = $"group-{unique}",
            Description = "Integration test group"
        });
        Assert.Equal(HttpStatusCode.Created, groupResponse.StatusCode);

        var createdGroup = await ReadResponseAsync<BaseResponse<GroupResponse>>(groupResponse);
        Assert.True(createdGroup.Success);
        var groupId = createdGroup.Data!.Id;

        var assignResponse = await client.PostAsJsonAsync($"/api/Groups/{groupId}/assign-role", new
        {
            RoleId = roleId
        });
        Assert.Equal(HttpStatusCode.OK, assignResponse.StatusCode);

        var assigned = await ReadResponseAsync<BaseResponse<GroupResponse>>(assignResponse);
        Assert.True(assigned.Success);
        Assert.Contains(createdRole.Data!.Name, assigned.Data!.Roles ?? Array.Empty<string>());

        var getResponse = await client.GetAsync($"/api/Groups/{groupId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await ReadResponseAsync<BaseResponse<GroupResponse>>(getResponse);
        Assert.True(fetched.Success);
        Assert.Contains(createdRole.Data!.Name, fetched.Data!.Roles ?? Array.Empty<string>());
    }

    /// <summary>
    /// Ensures users can be assigned to a group.
    /// </summary>
    [Fact]
    public async Task Groups_AssignUser_Works()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var unique = Guid.NewGuid().ToString("N");
        var userResponse = await client.PostAsJsonAsync("/api/Users", new
        {
            Username = $"group-user-{unique}",
            Email = $"group-user-{unique}@example.com",
            Password = "User123!"
        });
        Assert.Equal(HttpStatusCode.Created, userResponse.StatusCode);

        var createdUser = await ReadResponseAsync<BaseResponse<UserResponse>>(userResponse);
        Assert.True(createdUser.Success);
        var userId = createdUser.Data!.Id;

        var groupResponse = await client.PostAsJsonAsync("/api/Groups", new
        {
            Name = $"group-{unique}",
            Description = "Integration test group"
        });
        Assert.Equal(HttpStatusCode.Created, groupResponse.StatusCode);

        var createdGroup = await ReadResponseAsync<BaseResponse<GroupResponse>>(groupResponse);
        Assert.True(createdGroup.Success);
        var groupId = createdGroup.Data!.Id;

        var assignResponse = await client.PostAsJsonAsync($"/api/Groups/{groupId}/assign-user", new
        {
            UserId = userId
        });
        Assert.Equal(HttpStatusCode.OK, assignResponse.StatusCode);

        var assigned = await ReadResponseAsync<BaseResponse<GroupResponse>>(assignResponse);
        Assert.True(assigned.Success);

        var getResponse = await client.GetAsync($"/api/Groups/{groupId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await ReadResponseAsync<BaseResponse<GroupResponse>>(getResponse);
        Assert.True(fetched.Success);

        var getUserResponse = await client.GetAsync($"/api/Users/{userId}");
        Assert.Equal(HttpStatusCode.OK, getUserResponse.StatusCode);

        var fetchedUser = await ReadResponseAsync<BaseResponse<UserResponse>>(getUserResponse);
        Assert.True(fetchedUser.Success);
        Assert.Contains(createdGroup.Data!.Name, fetchedUser.Data!.Groups ?? Array.Empty<string>());
    }
}
