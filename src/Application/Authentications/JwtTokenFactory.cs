#nullable enable
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces;
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
    private const string DefaultRole = "User";

    public static JwtTokenResult Create(User user, IConfiguration configuration, IDateTime dateTime)
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

        var identity = string.IsNullOrWhiteSpace(user.Username) ? user.Email : user.Username;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, identity ?? user.Id.ToString())
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
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
            claims.Add(new Claim(ClaimTypes.Role, role!));
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
