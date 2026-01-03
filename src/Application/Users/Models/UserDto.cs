#nullable enable
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;

namespace Application.Users.Models;

/// <summary>
/// Represents a user summary for API responses.
/// </summary>
public sealed class UserDto
{
    public UserDto(User user)
    {
        Id = user.Id;
        Username = user.Username;
        Email = user.Email;
        PhoneNumber = user.PhoneNumber;
        EmailConfirm = user.EmailConfirm;
        PhoneNumberConfirm = user.PhoneNumberConfirm;
        TwoFactorEnabled = user.TwoFactorEnabled;
        AccessFailedCount = user.AccessFailedCount;
        IsLocked = user.IsLocked;
        IsDeleted = user.IsDeleted;
        Roles = user.UserRoles
            .Select(ur => ur.Role?.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        Groups = user.UserGroups
            .Select(ug => ug.Group?.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        GroupRoles = user.UserGroups
            .Where(ug => ug.Group != null && !string.IsNullOrWhiteSpace(ug.Group.Name))
            .GroupBy(ug => ug.Group!.Name!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyCollection<string>)g
                    .SelectMany(ug => ug.Group?.GroupRoles ?? new List<GroupRole>())
                    .Select(gr => gr.Role?.Name)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList(),
                StringComparer.OrdinalIgnoreCase);
        var directPermissions = user.UserRoles
            .SelectMany(ur => ur.Role?.RolePermissions ?? new List<RolePermission>())
            .Select(rp => rp.Permission?.Name);

        var groupPermissions = user.UserGroups
            .SelectMany(ug => ug.Group?.GroupRoles ?? new List<GroupRole>())
            .SelectMany(gr => gr.Role?.RolePermissions ?? new List<RolePermission>())
            .Select(rp => rp.Permission?.Name);

        Permissions = directPermissions
            .Concat(groupPermissions)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public Guid Id { get; }
    public string? Username { get; }
    public string? Email { get; }
    public string? PhoneNumber { get; }
    public bool EmailConfirm { get; }
    public bool PhoneNumberConfirm { get; }
    public bool TwoFactorEnabled { get; }
    public int AccessFailedCount { get; }
    public bool IsLocked { get; }
    public bool IsDeleted { get; }
    public IReadOnlyCollection<string> Roles { get; }
    public IReadOnlyCollection<string> Groups { get; }
    public IReadOnlyCollection<string> Permissions { get; }
    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> GroupRoles { get; }
}
