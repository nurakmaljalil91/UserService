#nullable enable
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces;
using Application.UserPreferences;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Authentications;

/// <summary>
/// Creates JWT tokens for authenticated users.
/// </summary>
internal static class JwtTokenFactory
{
    private const int DefaultExpiryMinutes = 60;
    private const int PersonalAccessTokenMaxExpiryMinutes = 15;
    private const string ScopeClaimType = "scope";
    private const string TokenUseClaimType = "token_use";
    private const string PersonalAccessTokenUse = "pat";
    private const string PersonalAccessTokenIdClaimType = "pat_id";
    private const string DefaultRole = "User";
    private const string RoleClaimType = "role";
    private const string PreferredUsernameClaimType = "preferred_username";
    private const string ZoneInfoClaimType = "zoneinfo";

    public static JwtTokenResult Create(
        User user,
        IConfiguration configuration,
        IDateTime dateTime,
        string? timeZone = null,
        PersonalAccessTokenGrant? grant = null)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var key = jwtSection["Key"];

        if (string.IsNullOrWhiteSpace(issuer) ||
            string.IsNullOrWhiteSpace(audience) ||
            string.IsNullOrWhiteSpace(key))
        {
            return JwtTokenResult.Fail("JWT configuration is missing.");
        }

        var expiryMinutes = DefaultExpiryMinutes;
        if (int.TryParse(jwtSection["ExpiryMinutes"], out var configuredMinutes) && configuredMinutes > 0)
        {
            expiryMinutes = configuredMinutes;
        }

        if (grant is not null)
        {
            expiryMinutes = Math.Min(expiryMinutes, PersonalAccessTokenMaxExpiryMinutes);
        }

        var preferredUsername = string.IsNullOrWhiteSpace(user.Username) ? user.Email : user.Username;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString())
        };

        if (!string.IsNullOrWhiteSpace(preferredUsername))
        {
            claims.Add(new Claim(PreferredUsernameClaimType, preferredUsername));
        }

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        }

        if (TimeZonePreference.IsValidValue(timeZone))
        {
            claims.Add(new Claim(ZoneInfoClaimType, timeZone!.Trim()));
        }

        var roles = user.UserRoles
            .Select(ur => ur.Role?.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (roles.Count == 0)
        {
            roles.Add(DefaultRole);
        }

        foreach (var role in roles)
        {
            claims.Add(new Claim(RoleClaimType, role!));
        }

        if (grant is not null)
        {
            claims.Add(new Claim(TokenUseClaimType, PersonalAccessTokenUse));
            claims.Add(new Claim(PersonalAccessTokenIdClaimType, grant.TokenId.ToString()));
            claims.AddRange(grant.Scopes.Select(scope => new Claim(ScopeClaimType, scope)));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expires = dateTime.UtcNow.AddMinutes(expiryMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return JwtTokenResult.Ok(tokenValue, expires);
    }
}

internal sealed record JwtTokenResult(
    bool Success,
    string? Error,
    string Token,
    DateTime ExpiresAt)
{
    public static JwtTokenResult Ok(string token, DateTime expiresAt) =>
        new(true, null, token, expiresAt);

    public static JwtTokenResult Fail(string error) =>
        new(false, error, string.Empty, DateTime.MinValue);
}

/// <summary>
/// The personal access token an access token is being issued for.
/// </summary>
/// <param name="TokenId">The personal access token identifier.</param>
/// <param name="Scopes">The scopes to embed, already expanded so that write implies read.</param>
internal sealed record PersonalAccessTokenGrant(Guid TokenId, IReadOnlyList<string> Scopes);
