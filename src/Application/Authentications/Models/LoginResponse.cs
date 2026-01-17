using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Authentications.Models;

/// <summary>
/// Represents the response returned after a successful login, containing the authentication token and its expiration time.
/// </summary>
public sealed record LoginResponse(
    string Token,
    DateTime ExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt
);
