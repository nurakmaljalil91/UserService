using System.Net;
using System.Net.Http.Json;
using Domain.Common;

namespace IntegrationTests;

[Collection("Integration")]
public class TodoItemsControllerIntegrationTests : ApiTestBase
{
    public TodoItemsControllerIntegrationTests(ApiFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task TodoItems_FullFlow_Works()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var getResponse = await client.GetAsync("/api/TodoItems");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var getPayload = await ReadResponseAsync<BaseResponse<PaginatedResponse<TodoItemResponse>>>(getResponse);
        Assert.True(getPayload.Success);

        var createResponse = await client.PostAsJsonAsync("/api/TodoItems", new
        {
            Title = "Integration Item"
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await ReadResponseAsync<BaseResponse<TodoItemResponse>>(createResponse);
        Assert.True(created.Success);
        Assert.NotNull(created.Data);

        var itemId = created.Data!.Id;

        var updateResponse = await client.PatchAsync("/api/TodoItems", JsonContent.Create(new
        {
            Id = itemId,
            Title = "Integration Item Updated"
        }));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await ReadResponseAsync<BaseResponse<TodoItemResponse>>(updateResponse);
        Assert.True(updated.Success);
        Assert.Equal("Integration Item Updated", updated.Data!.Title);

        var deleteResponse = await client.DeleteAsync($"/api/TodoItems/{itemId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var deleted = await ReadResponseAsync<BaseResponse<object>>(deleteResponse);
        Assert.True(deleted.Success);
    }
}
