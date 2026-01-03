using System.Net;
using System.Net.Http.Json;
using Domain.Common;

namespace IntegrationTests;

[Collection("Integration")]
public class TodoListsControllerIntegrationTests : ApiTestBase
{
    public TodoListsControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task TodoLists_FullFlow_Works()
    {
        using var client = CreateClient();

        var getResponse = await client.GetAsync("/api/TodoLists");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var getPayload = await ReadResponseAsync<BaseResponse<PaginatedResponse<TodoListResponse>>>(getResponse);
        Assert.True(getPayload.Success);

        var createResponse = await client.PostAsJsonAsync("/api/TodoLists", new
        {
            Title = "Integration List"
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<TodoListResponse>>(createResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        var listId = created.Data!.Id;

        var updateResponse = await client.PatchAsync("/api/TodoLists", JsonContent.Create(new
        {
            Id = listId,
            Title = "Integration List Updated"
        }));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await ReadResponseAsync<BaseResponse<TodoListResponse>>(updateResponse);
        Assert.True(updated.Success);
        Assert.Equal("Integration List Updated", updated.Data!.Title);

        var deleteResponse = await client.DeleteAsync($"/api/TodoLists/{listId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var deleted = await ReadResponseAsync<BaseResponse<string>>(deleteResponse);
        Assert.True(deleted.Success);
    }
}
