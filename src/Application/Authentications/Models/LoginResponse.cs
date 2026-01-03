using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Authentications.Models;

/// <summary>
/// Represents the response returned after a successful login, containing the authentication token and its expiration time.
/// </summary>
public sealed record LoginResponse(
    /// <summary>
    /// The authentication token issued to the user.
    /// </summary>
    string Token,
    /// <summary>
    /// The date and time when the token expires.
    /// </summary>
    DateTime ExpiresAt
);
