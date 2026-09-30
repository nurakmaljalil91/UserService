using System.Net;
using Domain.Common;

namespace IntegrationTests;

/// <summary>
/// Integration tests for language API registration and filtering.
/// </summary>
[Collection("Integration")]
public class LanguagesControllerIntegrationTests : ApiTestBase
{
    /// <summary>
    /// Initializes the language API tests.
    /// </summary>
    /// <param name="factory">The API factory.</param>
    public LanguagesControllerIntegrationTests(ApiFactory factory) : base(factory)
    {
    }

    /// <summary>
    /// Ensures a registered user can load their languages.
    /// </summary>
    [Fact]
    public async Task GetLanguages_ForRegisteredUser_ReturnsEmptyList()
    {
        var authenticated = await CreateAuthenticatedClientWithUserAsync();
        using var client = authenticated.Client;

        var response = await client.GetAsync($"/api/Languages?userId={authenticated.UserId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await ReadResponseAsync<BaseResponse<PaginatedResponse<object>>>(response);
        Assert.True(payload.Success);
        Assert.Empty(payload.Data!.Items!);
    }
}
