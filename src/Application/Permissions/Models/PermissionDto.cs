#nullable enable
using System;
using Domain.Entities;

namespace Application.Permissions.Models;

/// <summary>
/// Represents a permission summary for API responses.
/// </summary>
public sealed record PermissionDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionDto"/> class.
    /// </summary>
    public PermissionDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionDto"/> class from a <see cref="Permission"/> entity.
    /// </summary>
    /// <param name="permission">The <see cref="Permission"/> entity to map from.</param>
    public PermissionDto(Permission permission)
    {
        Id = permission.Id;
        Name = permission.Name;
        NormalizedName = permission.NormalizedName;
        Description = permission.Description;
    }

    /// <summary>
    /// Gets the permission identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the permission name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the normalized permission name.
    /// </summary>
    public string? NormalizedName { get; init; }

    /// <summary>
    /// Gets the permission description.
    /// </summary>
    public string? Description { get; init; }
}
