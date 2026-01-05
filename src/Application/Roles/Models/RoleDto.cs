#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;

namespace Application.Roles.Models;

/// <summary>
/// Represents a role summary for API responses.
/// </summary>
public sealed class RoleDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleDto"/> class with default values.
    /// </summary>
    public RoleDto()
    {
        Permissions = Array.Empty<string>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleDto"/> class from a <see cref="Role"/> entity.
    /// </summary>
    /// <param name="role">The <see cref="Role"/> entity to map from.</param>
    public RoleDto(Role role)
    {
        Id = role.Id;
        Name = role.Name;
        NormalizedName = role.NormalizedName;
        Description = role.Description;
        Permissions = role.RolePermissions
            .Select(rp => rp.Permission?.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? NormalizedName { get; init; }
    public string? Description { get; init; }
    public IReadOnlyCollection<string> Permissions { get; init; }
}
