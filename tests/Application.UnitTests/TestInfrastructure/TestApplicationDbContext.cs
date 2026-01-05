using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.TestInfrastructure;

/// <summary>
/// Represents a test implementation of <see cref="IApplicationDbContext"/> using Entity Framework Core for unit testing purposes.
/// </summary>
public sealed class TestApplicationDbContext : DbContext, IApplicationDbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestApplicationDbContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public TestApplicationDbContext(DbContextOptions<TestApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the <see cref="DbSet{TodoList}"/> representing the collection of task lists.
    /// </summary>
    public DbSet<TodoList> TodoLists => Set<TodoList>();

    /// <summary>
    /// Gets the <see cref="DbSet{TodoItem}"/> representing the collection of task items.
    /// </summary>
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public DbSet<User> Users => Set<User>();

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    public DbSet<Session> Sessions => Set<Session>();

    public DbSet<ContactMethod> ContactMethods => Set<ContactMethod>();

    public DbSet<Address> Addresses => Set<Address>();

    public DbSet<LoginAttempt> LoginAttempts => Set<LoginAttempt>();

    public DbSet<Consent> Consents => Set<Consent>();

    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<Group> Groups => Set<Group>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<UserGroup> UserGroups => Set<UserGroup>();

    public DbSet<GroupRole> GroupRoles => Set<GroupRole>();

    /// <summary>
    /// Configures the entity mappings for the context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoList>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.OwnsOne(x => x.Colour);
            builder.Navigation(x => x.Items)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<TodoItem>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.List)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.ListId);
        });

        modelBuilder.Entity<User>(builder => builder.HasKey(x => x.Id));
        modelBuilder.Entity<UserProfile>(builder => builder.HasKey(x => x.Id));
        modelBuilder.Entity<Session>(builder => builder.HasKey(x => x.Id));
        modelBuilder.Entity<ContactMethod>(builder => builder.HasKey(x => x.Id));
        modelBuilder.Entity<Address>(builder => builder.HasKey(x => x.Id));
        modelBuilder.Entity<LoginAttempt>(builder => builder.HasKey(x => x.Id));
        modelBuilder.Entity<Consent>(builder => builder.HasKey(x => x.Id));
        modelBuilder.Entity<UserPreference>(builder => builder.HasKey(x => x.Id));
        modelBuilder.Entity<Role>(builder => builder.HasKey(x => x.Id));
        modelBuilder.Entity<Permission>(builder => builder.HasKey(x => x.Id));
        modelBuilder.Entity<Group>(builder => builder.HasKey(x => x.Id));
        modelBuilder.Entity<UserRole>(builder => builder.HasKey(x => new { x.UserId, x.RoleId }));
        modelBuilder.Entity<RolePermission>(builder => builder.HasKey(x => new { x.RoleId, x.PermissionId }));
        modelBuilder.Entity<UserGroup>(builder => builder.HasKey(x => new { x.UserId, x.GroupId }));
        modelBuilder.Entity<GroupRole>(builder => builder.HasKey(x => new { x.GroupId, x.RoleId }));
    }
}
