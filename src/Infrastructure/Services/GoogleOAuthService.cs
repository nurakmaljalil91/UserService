
#nullable enable
using System.Net.Http.Headers;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Common.Interfaces;
using Application.ExternalLinks.Models;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

/// <summary>
/// Implements Google OAuth flows for external account linking.
/// </summary>
public sealed class GoogleOAuthService : IGoogleOAuthService
{
    private const string SettingsSection = "ExternalOAuth:Google";
    private const string ClientIdSetting = "ClientId";
    private const string ClientSecretSetting = "ClientSecret";
    private const string RedirectUriSetting = "RedirectUri";
    private const string AuthorizationEndpointSetting = "AuthorizationEndpoint";
    private const string TokenEndpointSetting = "TokenEndpoint";
    private const string UserInfoEndpointSetting = "UserInfoEndpoint";
    private const string ScopesSetting = "Scopes";
    private const string DefaultAuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
    private const string DefaultTokenEndpoint = "https://oauth2.googleapis.com/token";
    private const string DefaultUserInfoEndpoint = "https://openidconnect.googleapis.com/v1/userinfo";
    private const string CalendarScope = "https://www.googleapis.com/auth/calendar";
    private const string OpenIdScope = "openid";
    private const string EmailScope = "email";
    private const string PromptConsent = "consent";
    private const string AccessTypeOffline = "offline";
    private const string ResponseTypeCode = "code";
    private const string GrantTypeAuthorizationCode = "authorization_code";
    private const string GrantTypeRefreshToken = "refresh_token";

    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;
    private readonly string _authorizationEndpoint;
    private readonly string _tokenEndpoint;
    private readonly string _userInfoEndpoint;
    private readonly string _scopes;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleOAuthService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="configuration">The application configuration.</param>
    public GoogleOAuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        var section = configuration.GetSection(SettingsSection);
        _clientId = section[ClientIdSetting] ?? string.Empty;
        _clientSecret = section[ClientSecretSetting] ?? string.Empty;
        _redirectUri = section[RedirectUriSetting] ?? string.Empty;
        _authorizationEndpoint = section[AuthorizationEndpointSetting] ?? DefaultAuthorizationEndpoint;
        _tokenEndpoint = section[TokenEndpointSetting] ?? DefaultTokenEndpoint;
        _userInfoEndpoint = section[UserInfoEndpointSetting] ?? DefaultUserInfoEndpoint;

        var scopeValues = section.GetSection(ScopesSetting).Get<string[]>()
            ?? new[] { CalendarScope, OpenIdScope, EmailScope };
        _scopes = string.Join(" ", scopeValues.Where(scope => !string.IsNullOrWhiteSpace(scope)));
    }

    /// <inheritdoc />
    public async Task<string> BuildAuthorizationUrlAsync(string state, CancellationToken cancellationToken)
    {
        EnsureConfigured();

        var parameters = new Dictionary<string, string>
        {
            ["client_id"] = _clientId,
            ["redirect_uri"] = _redirectUri,
            ["response_type"] = ResponseTypeCode,
            ["scope"] = _scopes,
            ["state"] = state,
            ["access_type"] = AccessTypeOffline,
            ["prompt"] = PromptConsent,
            ["include_granted_scopes"] = "true"
        };

        var queryString = await CreateQueryStringAsync(parameters, cancellationToken);
        return $"{_authorizationEndpoint}?{queryString}";
    }

    /// <inheritdoc />
    public async Task<ExternalOAuthToken> ExchangeCodeAsync(string code, CancellationToken cancellationToken)
    {
        EnsureConfigured();

        var parameters = new Dictionary<string, string>
        {
            ["client_id"] = _clientId,
            ["client_secret"] = _clientSecret,
            ["code"] = code,
            ["grant_type"] = GrantTypeAuthorizationCode,
            ["redirect_uri"] = _redirectUri
        };

        using var content = new FormUrlEncodedContent(parameters);
        using var response = await _httpClient.PostAsync(_tokenEndpoint, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await ReadJsonAsync<GoogleTokenPayload>(response, cancellationToken);
        return new ExternalOAuthToken
        {
            AccessToken = payload.AccessToken ?? string.Empty,
            RefreshToken = payload.RefreshToken,
            ExpiresInSeconds = payload.ExpiresInSeconds,
            Scope = payload.Scope,
            TokenType = payload.TokenType
        };
    }

    /// <inheritdoc />
    public async Task<ExternalOAuthToken> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        EnsureConfigured();

        var parameters = new Dictionary<string, string>
        {
            ["client_id"] = _clientId,
            ["client_secret"] = _clientSecret,
            ["refresh_token"] = refreshToken,
            ["grant_type"] = GrantTypeRefreshToken
        };

        using var content = new FormUrlEncodedContent(parameters);
        using var response = await _httpClient.PostAsync(_tokenEndpoint, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await ReadJsonAsync<GoogleTokenPayload>(response, cancellationToken);
        return new ExternalOAuthToken
        {
            AccessToken = payload.AccessToken ?? string.Empty,
            RefreshToken = refreshToken,
            ExpiresInSeconds = payload.ExpiresInSeconds,
            Scope = payload.Scope,
            TokenType = payload.TokenType
        };
    }

    /// <inheritdoc />
    public async Task<ExternalOAuthUserProfile> GetUserProfileAsync(string accessToken, CancellationToken cancellationToken)
    {
        EnsureConfigured();

        using var request = new HttpRequestMessage(HttpMethod.Get, _userInfoEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await ReadJsonAsync<GoogleUserInfoPayload>(response, cancellationToken);
        return new ExternalOAuthUserProfile
        {
            SubjectId = payload.SubjectId ?? string.Empty,
            Email = payload.Email,
            DisplayName = payload.Name
        };
    }
    /// <summary>
    /// Ensures the Google OAuth configuration values are present.
    /// </summary>
    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_clientId) ||
            string.IsNullOrWhiteSpace(_clientSecret) ||
            string.IsNullOrWhiteSpace(_redirectUri))
        {
            throw new InvalidOperationException("Google OAuth configuration is missing.");
        }
    }

    /// <summary>
    /// Creates a URL-encoded query string from the provided parameters.
    /// </summary>
    /// <param name="parameters">The parameters to encode.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The encoded query string.</returns>
    private static async Task<string> CreateQueryStringAsync(
        IDictionary<string, string> parameters,
        CancellationToken cancellationToken)
    {
        using var content = new FormUrlEncodedContent(parameters);
        return await content.ReadAsStringAsync(cancellationToken);
    }

    /// <summary>
    /// Reads and deserializes the JSON content from an HTTP response.
    /// </summary>
    /// <typeparam name="T">The payload type.</typeparam>
    /// <param name="response">The HTTP response.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The deserialized payload.</returns>
    private static async Task<T> ReadJsonAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var payload = await JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }, cancellationToken);

        if (payload == null)
        {
            throw new InvalidOperationException("Failed to parse Google OAuth response.");
        }

        return payload;
    }

    /// <summary>
    /// Represents the Google token payload returned by the OAuth endpoint.
    /// </summary>
    private sealed class GoogleTokenPayload
    {
        /// <summary>
        /// Gets or sets the access token value.
        /// </summary>
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the refresh token value.
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the expiration duration in seconds.
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresInSeconds { get; set; }

        /// <summary>
        /// Gets or sets the granted scopes.
        /// </summary>
        [JsonPropertyName("scope")]
        public string? Scope { get; set; }

        /// <summary>
        /// Gets or sets the token type.
        /// </summary>
        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
    }

    /// <summary>
    /// Represents the Google user info payload returned by the user info endpoint.
    /// </summary>
    private sealed class GoogleUserInfoPayload
    {
        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        [JsonPropertyName("sub")]
        public string? SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
