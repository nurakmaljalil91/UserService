using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Text.Json;
using System.Linq;
using Application.Authentications.Models;
using Domain.Common;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests;

/// <summary>
/// Provides a base class for integration tests that interact with the web API, offering common setup and utility
/// methods for HTTP client creation and response handling.
/// </summary>
/// <remarks>This class is intended to be inherited by test classes that require consistent API client
/// configuration and authentication setup. It supplies helper methods for creating authenticated and unauthenticated
/// HTTP clients, as well as for deserializing API responses using standardized JSON serialization options. The class is
/// designed for use in test scenarios and is not intended for production use.</remarks>
public abstract class ApiTestBase
{
    /// <summary>
    /// Gets the default <see cref="JsonSerializerOptions"/> configured for web APIs.
    /// </summary>
    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ApiFactory _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiTestBase"/> class with the specified <see cref="ApiFactory"/>.
    /// </summary>
    /// <param name="factory">The <see cref="ApiFactory"/> used to create HTTP clients for integration testing.</param>
    protected ApiTestBase(ApiFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Creates a new <see cref="HttpClient"/> instance configured for integration testing with the API.
    /// </summary>
    /// <returns>A configured <see cref="HttpClient"/> for making API requests.</returns>
    protected HttpClient CreateClient()
        => _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });

    /// <summary>
    /// Creates a new <see cref="HttpClient"/> instance with a valid authentication token for integration testing.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, with a configured <see cref="HttpClient"/>
    /// that includes a Bearer token in the Authorization header.
    /// </returns>
    protected async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = CreateClient();
        var token = await GetTokenAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    /// <summary>
    /// Creates a new <see cref="HttpClient"/> instance with a valid authentication token and returns the user id.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, with the client and user identifier.
    /// </returns>
    protected async Task<(HttpClient Client, Guid UserId)> CreateAuthenticatedClientWithUserAsync()
    {
        var client = CreateClient();
        var token = await GetTokenAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var subject = jwt.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(subject, out var userId))
        {
            throw new InvalidOperationException("Token does not contain a valid subject claim.");
        }

        return (client, userId);
    }

    /// <summary>
    /// Reads and deserializes the HTTP response content as the specified type using the configured JSON options.
    /// </summary>
    /// <typeparam name="T">The type to which the response content should be deserialized.</typeparam>
    /// <param name="response">The <see cref="HttpResponseMessage"/> containing the response content.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, with the deserialized response payload.</returns>
    protected static async Task<T> ReadResponseAsync<T>(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        Assert.NotNull(payload);
        return payload!;
    }

    private static async Task<string> GetTokenAsync(HttpClient client)
    {
        var unique = Guid.NewGuid().ToString("N");
        var username = $"integration-user-{unique}";
        var email = $"integration-{unique}@example.com";
        const string password = "Integration123!";

        var register = new
        {
            Username = username,
            Email = email,
            Password = password
        };

        var registerResponse = await client.PostAsJsonAsync("/api/Authentications/register", register);
        registerResponse.EnsureSuccessStatusCode();

        var login = new
        {
            Username = username,
            Email = email,
            Password = password
        };

        var response = await client.PostAsJsonAsync("/api/Authentications/login", login);
        response.EnsureSuccessStatusCode();

        var payload = await ReadResponseAsync<BaseResponse<LoginResponse>>(response);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Data);
        Assert.False(string.IsNullOrWhiteSpace(payload.Data!.Token));

        return payload.Data!.Token;
    }

#nullable enable

    /// <summary>
    /// Represents a paginated response containing a collection of items and pagination metadata.
    /// </summary>
    /// <typeparam name="T">The type of items contained in the paginated response.</typeparam>
    protected sealed class PaginatedResponse<T>
    {
        /// <summary>
        /// Gets or sets the collection of items in the current page.
        /// </summary>
        public IEnumerable<T>? Items { get; set; }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages available.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the total count of items across all pages.
        /// </summary>
        public int TotalCount { get; set; }
    }

#nullable restore

    /// <summary>
    /// Represents a response containing details of a single to-do item.
    /// </summary>
    protected sealed class TodoItemResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the to-do item.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets the identifier of the to-do list to which the item belongs.
        /// </summary>
        public long ListId { get; set; }
        /// <summary>
        /// Gets or sets the title of the to-do item.
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the to-do item is marked as done.
        /// </summary>
        public bool Done { get; set; }
    }

    /// <summary>
    /// Represents a response containing details of a single to-do list, including its items.
    /// </summary>
    protected sealed class TodoListResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the to-do list.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets the title of the to-do list.
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// Gets or sets the colour of the to-do list.
        /// </summary>
        public string? Colour { get; set; }
        /// <summary>
        /// Gets or sets the collection of to-do items belonging to the to-do list.
        /// </summary>
        public IReadOnlyCollection<TodoItemResponse>? Items { get; set; }
    }

    /// <summary>
    /// Represents a response containing details of a user.
    /// </summary>
    protected sealed class UserResponse
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool EmailConfirm { get; set; }
        public bool PhoneNumberConfirm { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDeleted { get; set; }
        public IReadOnlyCollection<string>? Roles { get; set; }
        public IReadOnlyCollection<string>? Groups { get; set; }
        public IReadOnlyCollection<string>? Permissions { get; set; }
        public IReadOnlyDictionary<string, IReadOnlyCollection<string>>? GroupRoles { get; set; }
    }

    /// <summary>
    /// Represents a response containing details of a role.
    /// </summary>
    protected sealed class RoleResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? NormalizedName { get; set; }
        public string? Description { get; set; }
        public IReadOnlyCollection<string>? Permissions { get; set; }
    }

#nullable enable

    /// <summary>
    /// Represents a response containing details of a group.
    /// </summary>
    protected sealed class GroupResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the group.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the group name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the normalized group name.
        /// </summary>
        public string? NormalizedName { get; set; }

        /// <summary>
        /// Gets or sets the group description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the group role names.
        /// </summary>
        public IReadOnlyCollection<string>? Roles { get; set; }
    }

    /// <summary>
    /// Represents a response containing details of a user profile.
    /// </summary>
    protected sealed class UserProfileResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user profile.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the display name shown to other users.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the national identity card number.
        /// </summary>
        public string? IdentityCardNumber { get; set; }

        /// <summary>
        /// Gets or sets the passport number.
        /// </summary>
        public string? PassportNumber { get; set; }

        /// <summary>
        /// Gets or sets the date of birth in yyyy-MM-dd format.
        /// </summary>
        public string? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the place of birth.
        /// </summary>
        public string? BirthPlace { get; set; }

        /// <summary>
        /// Gets or sets the shoe size.
        /// </summary>
        public string? ShoeSize { get; set; }

        /// <summary>
        /// Gets or sets the clothing size.
        /// </summary>
        public string? ClothingSize { get; set; }

        /// <summary>
        /// Gets or sets the waist size.
        /// </summary>
        public string? WaistSize { get; set; }

        /// <summary>
        /// Gets or sets the profile bio.
        /// </summary>
        public string? Bio { get; set; }

        /// <summary>
        /// Gets or sets the profile image URL.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the profile tag.
        /// </summary>
        public string? Tag { get; set; }

        /// <summary>
        /// Gets or sets the blood type recorded for the user.
        /// </summary>
        public string? BloodType { get; set; }
    }

    /// <summary>
    /// Represents a response containing details of a user preference.
    /// </summary>
    protected sealed class UserPreferenceResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user preference.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the preference key.
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// Gets or sets the preference value.
        /// </summary>
        public string? Value { get; set; }
    }

    /// <summary>
    /// Represents a response containing details of a session.
    /// </summary>
    protected sealed class SessionResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the session.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the refresh token for the session.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the expiration date for the session in UTC.
        /// </summary>
        public string? ExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets the time the session was revoked in UTC.
        /// </summary>
        public string? RevokedAt { get; set; }

        /// <summary>
        /// Gets or sets the IP address associated with the session.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the user agent associated with the session.
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the device name associated with the session.
        /// </summary>
        public string? DeviceName { get; set; }

        /// <summary>
        /// Gets or sets whether the session is revoked.
        /// </summary>
        public bool IsRevoked { get; set; }
    }

    /// <summary>
    /// Represents a response containing details of an address.
    /// </summary>
    protected sealed class AddressResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the address.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the address label.
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// Gets or sets the address type.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the first address line.
        /// </summary>
        public string? Line1 { get; set; }

        /// <summary>
        /// Gets or sets the second address line.
        /// </summary>
        public string? Line2 { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the state or region.
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        public string? PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Gets or sets whether the address is default.
        /// </summary>
        public bool IsDefault { get; set; }
    }

    /// <summary>
    /// Represents a response containing details of a consent.
    /// </summary>
    protected sealed class ConsentResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the consent.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the consent type.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets whether the consent is granted.
        /// </summary>
        public bool IsGranted { get; set; }

        /// <summary>
        /// Gets or sets when the consent was granted or revoked in UTC.
        /// </summary>
        public string? GrantedAt { get; set; }

        /// <summary>
        /// Gets or sets the consent version.
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// Gets or sets the consent source.
        /// </summary>
        public string? Source { get; set; }
    }

    /// <summary>
    /// Represents a response containing details of a contact method.
    /// </summary>
    protected sealed class ContactMethodResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the contact method.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the contact method type.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the contact value.
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Gets or sets the normalized contact value.
        /// </summary>
        public string? NormalizedValue { get; set; }

        /// <summary>
        /// Gets or sets whether the contact method is verified.
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Gets or sets whether the contact method is primary.
        /// </summary>
        public bool IsPrimary { get; set; }
    }

#nullable restore

    /// <summary>
    /// Represents a response containing details of a permission.
    /// </summary>
    protected sealed class PermissionResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the permission.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the permission name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the normalized permission name.
        /// </summary>
        public string? NormalizedName { get; set; }

        /// <summary>
        /// Gets or sets the permission description.
        /// </summary>
        public string? Description { get; set; }
    }
}
