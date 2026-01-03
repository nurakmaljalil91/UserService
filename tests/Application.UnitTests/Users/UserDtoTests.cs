#nullable enable
using Application.Users.Models;
using Domain.Entities;

namespace Application.UnitTests.Users;

/// <summary>
/// Contains unit tests for <see cref="UserDto"/>.
/// </summary>
public class UserDtoTests
{
    [Fact]
    public void Constructor_PopulatesRolesGroupsAndPermissions()
    {
        var readPermission = new Permission { Name = "Users.Read", NormalizedName = "USERS.READ" };
        var writePermission = new Permission { Name = "Users.Write", NormalizedName = "USERS.WRITE" };

        var adminRole = new Role { Name = "Admin", NormalizedName = "ADMIN" };
        var userRole = new Role { Name = "User", NormalizedName = "USER" };

        adminRole.RolePermissions.Add(new RolePermission { Role = adminRole, Permission = writePermission });
        userRole.RolePermissions.Add(new RolePermission { Role = userRole, Permission = readPermission });

        var adminGroup = new Group { Name = "Admins", NormalizedName = "ADMINS" };
        adminGroup.GroupRoles.Add(new GroupRole { Group = adminGroup, Role = adminRole });

        var user = new User
        {
            Username = "user",
            NormalizedUsername = "USER",
            Email = "user@example.com",
            NormalizedEmail = "USER@EXAMPLE.COM"
        };

        user.UserRoles.Add(new UserRole { User = user, Role = userRole });
        user.UserGroups.Add(new UserGroup { User = user, Group = adminGroup });

        var dto = new UserDto(user);

        Assert.Contains("User", dto.Roles);
        Assert.Contains("Admins", dto.Groups);
        Assert.Contains("Users.Read", dto.Permissions);
        Assert.Contains("Users.Write", dto.Permissions);
        Assert.True(dto.GroupRoles.TryGetValue("Admins", out var groupRoles));
        Assert.Contains("Admin", groupRoles!);
    }
}
