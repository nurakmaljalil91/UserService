using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Infrastructure.Data;

/// <summary>
/// Provides methods to initialise and seed the application's database context.
/// In development the database is dropped and recreated on every startup.
/// In all other environments (staging, production) pending EF Core migrations are applied.
/// </summary>
public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContextInitialiser"/> class.
    /// </summary>
    /// <param name="logger">The logger to use for logging database initialisation events.</param>
    /// <param name="context">The application's database context.</param>
    /// <param name="passwordHasher">The password hashing service.</param>
    /// <param name="environment">The host environment used to determine initialisation strategy.</param>
    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context,
        IPasswordHasherService passwordHasher,
        IHostEnvironment environment)
    {
        _logger = logger;
        _context = context;
        _passwordHasher = passwordHasher;
        _environment = environment;
    }

    /// <summary>
    /// Initialises the database according to the current environment.
    /// In development the database is deleted and recreated from the current model.
    /// In all other environments all pending EF Core migrations are applied.
    /// </summary>
    public async Task InitialiseAsync()
    {
        try
        {
            if (_environment.IsDevelopment())
            {
                // Drop and recreate for a clean dev environment on every startup.
                // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
                await _context.Database.EnsureDeletedAsync();
                await _context.Database.EnsureCreatedAsync();
            }
            else
            {
                // Apply any pending migrations on startup. Migration files are compiled
                // into the app — no external DB access or manual dotnet ef commands needed
                // in production.
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
        }
    }

    /// <summary>
    /// Seeds the application's database with initial data.
    /// Essential data (roles, permissions, groups, admin user and their assignments) is seeded
    /// in all environments. Development-only data (standard user account, profiles, skills,
    /// education, work experiences, projects, languages, addresses, contacts, consents, and
    /// preferences) is seeded only when running in the Development environment.
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            await SeedEssentialAsync();

            if (_environment.IsDevelopment())
            {
                await TrySeedAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    /// <summary>
    /// Seeds the essential data required for the application to function in any environment.
    /// This includes the Admin and User roles, all permissions with their role assignments,
    /// the Administrators and Default groups with their role assignments, and the admin user
    /// with their role and group memberships.
    /// All operations are idempotent — they check for existing records before inserting.
    /// </summary>
    public async Task SeedEssentialAsync()
    {
        if (!await _context.Roles.AnyAsync())
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

        if (!await _context.Permissions.AnyAsync())
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

        if (!await _context.Groups.AnyAsync())
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

        var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == "ADMIN");
        var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == "USER");
        var adminGroup = await _context.Groups.FirstOrDefaultAsync(g => g.NormalizedName == "ADMINISTRATORS");
        var defaultGroup = await _context.Groups.FirstOrDefaultAsync(g => g.NormalizedName == "DEFAULT");

        if (!await _context.RolePermissions.AnyAsync() && adminRole != null && userRole != null)
        {
            var permissions = await _context.Permissions.ToListAsync();
            var readPermissions = permissions
                .Where(p => p.NormalizedName != null && p.NormalizedName.EndsWith(".READ"))
                .ToList();

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

        if (!await _context.GroupRoles.AnyAsync() && adminGroup != null && defaultGroup != null && adminRole != null && userRole != null)
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

        if (!await _context.Users.AnyAsync(u => u.NormalizedUsername == "ADMIN"))
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

            _context.Users.Add(admin);
            await _context.SaveChangesAsync();
        }

        var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUsername == "ADMIN");

        if (adminUser != null && adminRole != null && !await _context.UserRoles.AnyAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id))
        {
            _context.UserRoles.Add(new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            });

            await _context.SaveChangesAsync();
        }

        if (adminUser != null && adminGroup != null && !await _context.UserGroups.AnyAsync(ug => ug.UserId == adminUser.Id && ug.GroupId == adminGroup.Id))
        {
            _context.UserGroups.Add(new UserGroup
            {
                UserId = adminUser.Id,
                GroupId = adminGroup.Id
            });

            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Seeds development-only data: the standard "user" account and all associated profile
    /// data including skills, education, work experiences, projects, languages, addresses,
    /// contact methods, consents, and preferences for both the admin and standard user.
    /// This method is called only in the Development environment.
    /// All operations are idempotent — they check for existing records before inserting.
    /// </summary>
    public async Task TrySeedAsync()
    {
        if (!await _context.Users.AnyAsync(u => u.NormalizedUsername == "USER"))
        {
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

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUsername == "ADMIN");
        var standardUser = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUsername == "USER");
        var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == "USER");
        var defaultGroup = await _context.Groups.FirstOrDefaultAsync(g => g.NormalizedName == "DEFAULT");

        var testersGroup = await _context.Groups.FirstOrDefaultAsync(g => g.NormalizedName == "TESTERS");
        if (testersGroup == null)
        {
            testersGroup = new Group
            {
                Name = "Testers",
                NormalizedName = "TESTERS",
                Description = "Quality assurance group"
            };
            _context.Groups.Add(testersGroup);
            await _context.SaveChangesAsync();
        }

        if (standardUser != null && userRole != null && !await _context.UserRoles.AnyAsync(ur => ur.UserId == standardUser.Id && ur.RoleId == userRole.Id))
        {
            _context.UserRoles.Add(new UserRole
            {
                UserId = standardUser.Id,
                RoleId = userRole.Id
            });

            await _context.SaveChangesAsync();
        }

        if (standardUser != null && defaultGroup != null && !await _context.UserGroups.AnyAsync(ug => ug.UserId == standardUser.Id && ug.GroupId == defaultGroup.Id))
        {
            _context.UserGroups.Add(new UserGroup
            {
                UserId = standardUser.Id,
                GroupId = defaultGroup.Id
            });

            await _context.SaveChangesAsync();
        }

        if (testersGroup != null && standardUser != null && !await _context.UserGroups.AnyAsync(ug => ug.UserId == standardUser.Id && ug.GroupId == testersGroup.Id))
        {
            _context.UserGroups.Add(new UserGroup
            {
                UserId = standardUser.Id,
                GroupId = testersGroup.Id
            });
            await _context.SaveChangesAsync();
        }

        if (adminUser != null && !await _context.UserProfiles.AnyAsync(p => p.UserId == adminUser.Id))
        {
            _context.UserProfiles.Add(new UserProfile
            {
                UserId = adminUser.Id,
                DisplayName = "Admin Profile",
                FirstName = "System",
                LastName = "Admin",
                DateOfBirth = new LocalDate(1988, 5, 10),
                BirthPlace = "Kuala Lumpur",
                Bio = "Seeded admin profile",
                BloodType = "O+",
                Tag = "admin"
            });
            await _context.SaveChangesAsync();
        }

        if (standardUser != null && !await _context.UserProfiles.AnyAsync(p => p.UserId == standardUser.Id))
        {
            _context.UserProfiles.Add(new UserProfile
            {
                UserId = standardUser.Id,
                DisplayName = "User Profile",
                FirstName = "Standard",
                LastName = "User",
                DateOfBirth = new LocalDate(1995, 9, 21),
                BirthPlace = "Penang",
                Bio = "Seeded user profile",
                BloodType = "A+",
                Tag = "user"
            });
            await _context.SaveChangesAsync();
        }

        if (!await _context.Addresses.AnyAsync() && adminUser != null && standardUser != null)
        {
            _context.Addresses.AddRange(
                new Address
                {
                    UserId = adminUser.Id,
                    Label = "HQ",
                    Type = "work",
                    Line1 = "1 Admin Plaza",
                    City = "Kuala Lumpur",
                    State = "WP Kuala Lumpur",
                    PostalCode = "50000",
                    Country = "Malaysia",
                    IsDefault = true
                },
                new Address
                {
                    UserId = standardUser.Id,
                    Label = "Home",
                    Type = "home",
                    Line1 = "123 User Street",
                    City = "George Town",
                    State = "Penang",
                    PostalCode = "10000",
                    Country = "Malaysia",
                    IsDefault = true
                });

            await _context.SaveChangesAsync();
        }

        if (!await _context.ContactMethods.AnyAsync() && adminUser != null && standardUser != null)
        {
            _context.ContactMethods.AddRange(
                new ContactMethod
                {
                    UserId = adminUser.Id,
                    Type = "email",
                    Value = "admin@example.com",
                    NormalizedValue = "ADMIN@EXAMPLE.COM",
                    IsVerified = true,
                    IsPrimary = true
                },
                new ContactMethod
                {
                    UserId = standardUser.Id,
                    Type = "phone",
                    Value = "+60123456789",
                    NormalizedValue = "+60123456789",
                    IsVerified = true,
                    IsPrimary = true
                });

            await _context.SaveChangesAsync();
        }

        if (!await _context.Consents.AnyAsync() && adminUser != null && standardUser != null)
        {
            var grantedAt = Instant.FromUtc(2024, 1, 1, 0, 0);
            _context.Consents.AddRange(
                new Consent
                {
                    UserId = adminUser.Id,
                    Type = "TermsOfService",
                    IsGranted = true,
                    GrantedAt = grantedAt,
                    Version = "1.0",
                    Source = "seed"
                },
                new Consent
                {
                    UserId = standardUser.Id,
                    Type = "MarketingEmails",
                    IsGranted = false,
                    GrantedAt = grantedAt,
                    Version = "1.0",
                    Source = "seed"
                });

            await _context.SaveChangesAsync();
        }

        if (!await _context.UserPreferences.AnyAsync() && adminUser != null && standardUser != null)
        {
            _context.UserPreferences.AddRange(
                new UserPreference
                {
                    UserId = adminUser.Id,
                    Key = "theme",
                    Value = "light"
                },
                new UserPreference
                {
                    UserId = standardUser.Id,
                    Key = "language",
                    Value = "en"
                });

            await _context.SaveChangesAsync();
        }

        if (adminUser != null && !await _context.Skills.AnyAsync(s => s.UserId == adminUser.Id))
        {
            _context.Skills.AddRange(
                new Skill { UserId = adminUser.Id, Name = "Leadership", Proficiency = "Expert", YearsOfExperience = 10 },
                new Skill { UserId = adminUser.Id, Name = "Architecture", Proficiency = "Expert", YearsOfExperience = 8 },
                new Skill { UserId = adminUser.Id, Name = "C#", Proficiency = "Expert", YearsOfExperience = 12 });
            await _context.SaveChangesAsync();
        }

        if (standardUser != null && !await _context.Skills.AnyAsync(s => s.UserId == standardUser.Id))
        {
            _context.Skills.AddRange(
                new Skill { UserId = standardUser.Id, Name = "Angular", Proficiency = "Intermediate", YearsOfExperience = 3 },
                new Skill { UserId = standardUser.Id, Name = "TypeScript", Proficiency = "Intermediate", YearsOfExperience = 3 },
                new Skill { UserId = standardUser.Id, Name = "Communication", Proficiency = "Beginner", YearsOfExperience = 1 });
            await _context.SaveChangesAsync();
        }

        if (adminUser != null && !await _context.Educations.AnyAsync(e => e.UserId == adminUser.Id))
        {
            _context.Educations.Add(new Education
            {
                UserId = adminUser.Id,
                Institution = "MIT",
                Degree = "Master of Science",
                FieldOfStudy = "Computer Science",
                StartDate = new LocalDate(2010, 9, 1),
                EndDate = new LocalDate(2012, 6, 30)
            });
            await _context.SaveChangesAsync();
        }

        if (standardUser != null && !await _context.Educations.AnyAsync(e => e.UserId == standardUser.Id))
        {
            _context.Educations.Add(new Education
            {
                UserId = standardUser.Id,
                Institution = "University of Malaya",
                Degree = "Bachelor of Computer Science",
                FieldOfStudy = "Software Engineering",
                StartDate = new LocalDate(2013, 9, 1),
                EndDate = new LocalDate(2017, 6, 30)
            });
            await _context.SaveChangesAsync();
        }

        if (adminUser != null && !await _context.WorkExperiences.AnyAsync(w => w.UserId == adminUser.Id))
        {
            _context.WorkExperiences.Add(new WorkExperience
            {
                UserId = adminUser.Id,
                Company = "Cerxos",
                Position = "CTO",
                StartDate = new LocalDate(2020, 1, 1),
                EndDate = null,
                Location = "Kuala Lumpur"
            });
            await _context.SaveChangesAsync();
        }

        if (standardUser != null && !await _context.WorkExperiences.AnyAsync(w => w.UserId == standardUser.Id))
        {
            _context.WorkExperiences.Add(new WorkExperience
            {
                UserId = standardUser.Id,
                Company = "Tech Corp",
                Position = "Frontend Developer",
                StartDate = new LocalDate(2018, 3, 1),
                EndDate = new LocalDate(2023, 12, 31),
                Location = "Penang"
            });
            await _context.SaveChangesAsync();
        }

        if (adminUser != null && !await _context.Projects.AnyAsync(p => p.UserId == adminUser.Id))
        {
            _context.Projects.Add(new Project
            {
                UserId = adminUser.Id,
                Title = "CerxosWebSystem",
                TechStack = ".NET, Angular, PostgreSQL",
                StartDate = new LocalDate(2023, 1, 1),
                EndDate = null
            });
            await _context.SaveChangesAsync();
        }

        if (standardUser != null && !await _context.Projects.AnyAsync(p => p.UserId == standardUser.Id))
        {
            _context.Projects.Add(new Project
            {
                UserId = standardUser.Id,
                Title = "Portfolio Site",
                TechStack = "Angular, TailwindCSS",
                StartDate = new LocalDate(2022, 6, 1),
                EndDate = new LocalDate(2022, 12, 31)
            });
            await _context.SaveChangesAsync();
        }

        if (adminUser != null && !await _context.Languages.AnyAsync(l => l.UserId == adminUser.Id))
        {
            _context.Languages.AddRange(
                new Language { UserId = adminUser.Id, Name = "English", Level = "Native" },
                new Language { UserId = adminUser.Id, Name = "Malay", Level = "Fluent" });
            await _context.SaveChangesAsync();
        }

        if (standardUser != null && !await _context.Languages.AnyAsync(l => l.UserId == standardUser.Id))
        {
            _context.Languages.AddRange(
                new Language { UserId = standardUser.Id, Name = "Malay", Level = "Native" },
                new Language { UserId = standardUser.Id, Name = "English", Level = "Work Proficiency" });
            await _context.SaveChangesAsync();
        }
    }
}
