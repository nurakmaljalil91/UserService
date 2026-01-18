#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;

namespace Application.Groups.Models;

/// <summary>
/// Represents a group summary for API responses.
/// </summary>
public sealed record GroupDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupDto"/> class with default values.
    /// </summary>
    public GroupDto()
    {
        Roles = Array.Empty<string>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupDto"/> class from a <see cref="Group"/> entity.
    /// </summary>
    /// <param name="group">The <see cref="Group"/> entity to map from.</param>
    public GroupDto(Group group)
    {
        Id = group.Id;
        Name = group.Name;
        NormalizedName = group.NormalizedName;
        Description = group.Description;
        Roles = group.GroupRoles
            .Select(gr => gr.Role?.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// Gets the group identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the group name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the normalized group name.
    /// </summary>
    public string? NormalizedName { get; init; }

    /// <summary>
    /// Gets the group description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the group roles by name.
    /// </summary>
    public IReadOnlyCollection<string> Roles { get; init; }
}
