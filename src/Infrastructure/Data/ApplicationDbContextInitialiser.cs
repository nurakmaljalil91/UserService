using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data;

/// <summary>
/// Provides methods to initialise and seed the application's database context.
/// </summary>
public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContextInitialiser"/> class.
    /// </summary>
    /// <param name="logger">The logger to use for logging database initialisation events.</param>
    /// <param name="context">The application's database context.</param>
    /// <param name="passwordHasher">The password hashing service.</param>
    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context,
        IPasswordHasherService passwordHasher)
    {
        _logger = logger;
        _context = context;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Ensures that the application's database is deleted and created anew.
    /// </summary>
    public async Task InitialiseAsync()
    {
        try
        {
            // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
        }
    }

    /// <summary>
    /// Seeds the application's database with initial data if necessary.
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    /// <summary>
    /// Attempts to seed the application's database with default data if necessary.
    /// </summary>
    public async Task TrySeedAsync()
    {
        if (!_context.Users.Any())
        {
            var admin = new User
            {
                Username = "admin",
                NormalizedUsername = "ADMIN",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirm = true,
                PhoneNumberConfirm = false,
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
                IsLocked = false,
                IsDeleted = false
            };
            admin.PasswordHash = _passwordHasher.HashPassword(admin, "Admin123#");

            var user = new User
            {
                Username = "user",
                NormalizedUsername = "USER",
                Email = "user@example.com",
                NormalizedEmail = "USER@EXAMPLE.COM",
                EmailConfirm = true,
                PhoneNumberConfirm = false,
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
                IsLocked = false,
                IsDeleted = false
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, "User123#");

            _context.Users.AddRange(admin, user);

            await _context.SaveChangesAsync();
        }

        if (!_context.Roles.Any())
        {
            _context.Roles.AddRange(
                new Role
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Description = "System administrator"
                },
                new Role
                {
                    Name = "User",
                    NormalizedName = "USER",
                    Description = "Standard user"
                });

            await _context.SaveChangesAsync();
        }

        if (!_context.Permissions.Any())
        {
            _context.Permissions.AddRange(
                new Permission
                {
                    Name = "Users.Read",
                    NormalizedName = "USERS.READ",
                    Description = "Read user data"
                },
                new Permission
                {
                    Name = "Users.Write",
                    NormalizedName = "USERS.WRITE",
                    Description = "Manage user data"
                },
                new Permission
                {
                    Name = "Groups.Read",
                    NormalizedName = "GROUPS.READ",
                    Description = "Read group data"
                },
                new Permission
                {
                    Name = "Groups.Write",
                    NormalizedName = "GROUPS.WRITE",
                    Description = "Manage groups"
                },
                new Permission
                {
                    Name = "Roles.Read",
                    NormalizedName = "ROLES.READ",
                    Description = "Read role data"
                },
                new Permission
                {
                    Name = "Roles.Write",
                    NormalizedName = "ROLES.WRITE",
                    Description = "Manage roles"
                });

            await _context.SaveChangesAsync();
        }

        if (!_context.Groups.Any())
        {
            _context.Groups.AddRange(
                new Group
                {
                    Name = "Administrators",
                    NormalizedName = "ADMINISTRATORS",
                    Description = "Administrative group"
                },
                new Group
                {
                    Name = "Default",
                    NormalizedName = "DEFAULT",
                    Description = "Default group for users"
                });

            await _context.SaveChangesAsync();
        }

        var adminUser = _context.Users.FirstOrDefault(u => u.NormalizedUsername == "ADMIN");
        var standardUser = _context.Users.FirstOrDefault(u => u.NormalizedUsername == "USER");
        var adminRole = _context.Roles.FirstOrDefault(r => r.NormalizedName == "ADMIN");
        var userRole = _context.Roles.FirstOrDefault(r => r.NormalizedName == "USER");
        var adminGroup = _context.Groups.FirstOrDefault(g => g.NormalizedName == "ADMINISTRATORS");
        var defaultGroup = _context.Groups.FirstOrDefault(g => g.NormalizedName == "DEFAULT");

        if (!_context.RolePermissions.Any() && adminRole != null && userRole != null)
        {
            var permissions = _context.Permissions.ToList();
            var readPermissions = permissions.Where(p => p.NormalizedName != null && p.NormalizedName.EndsWith(".READ")).ToList();

            foreach (var permission in permissions)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = permission.Id
                });
            }

            foreach (var permission in readPermissions)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = userRole.Id,
                    PermissionId = permission.Id
                });
            }

            await _context.SaveChangesAsync();
        }

        if (!_context.GroupRoles.Any() && adminGroup != null && defaultGroup != null && adminRole != null && userRole != null)
        {
            _context.GroupRoles.AddRange(
                new GroupRole
                {
                    GroupId = adminGroup.Id,
                    RoleId = adminRole.Id
                },
                new GroupRole
                {
                    GroupId = defaultGroup.Id,
                    RoleId = userRole.Id
                });

            await _context.SaveChangesAsync();
        }

        if (!_context.UserRoles.Any() && adminUser != null && standardUser != null && adminRole != null && userRole != null)
        {
            _context.UserRoles.AddRange(
                new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                },
                new UserRole
                {
                    UserId = standardUser.Id,
                    RoleId = userRole.Id
                });

            await _context.SaveChangesAsync();
        }

        if (!_context.UserGroups.Any() && adminUser != null && standardUser != null && adminGroup != null && defaultGroup != null)
        {
            _context.UserGroups.AddRange(
                new UserGroup
                {
                    UserId = adminUser.Id,
                    GroupId = adminGroup.Id
                },
                new UserGroup
                {
                    UserId = standardUser.Id,
                    GroupId = defaultGroup.Id
                });

            await _context.SaveChangesAsync();
        }
    }
}
