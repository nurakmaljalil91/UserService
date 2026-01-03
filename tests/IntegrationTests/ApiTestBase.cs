using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Domain.Common;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests;

public abstract class ApiTestBase
{
    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ApiFactory _factory;

    protected ApiTestBase(ApiFactory factory)
    {
        _factory = factory;
    }

    protected HttpClient CreateClient()
        => _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });

    protected async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = CreateClient();
        var token = await GetTokenAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    protected static async Task<T> ReadResponseAsync<T>(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        Assert.NotNull(payload);
        return payload!;
    }

    private static async Task<string> GetTokenAsync(HttpClient client)
    {
        var login = new
        {
            Username = "integration-user",
            Email = "integration@example.com"
        };

        var response = await client.PostAsJsonAsync("/api/Auth/login", login);
        response.EnsureSuccessStatusCode();

        var payload = await ReadResponseAsync<BaseResponse<AuthTokenResponse>>(response);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Data);
        Assert.False(string.IsNullOrWhiteSpace(payload.Data!.Token));

        return payload.Data!.Token;
    }

    protected sealed record AuthTokenResponse(string Token, DateTime ExpiresAtUtc);

    protected sealed class PaginatedResponse<T>
    {
        public IEnumerable<T>? Items { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }

    protected sealed class TodoItemResponse
    {
        public long Id { get; set; }
        public long ListId { get; set; }
        public string? Title { get; set; }
        public bool Done { get; set; }
    }

    protected sealed class TodoListResponse
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Colour { get; set; }
        public IReadOnlyCollection<TodoItemResponse>? Items { get; set; }
    }
}
