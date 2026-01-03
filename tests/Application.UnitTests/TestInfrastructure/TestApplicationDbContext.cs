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
    /// Gets the <see cref="DbSet{TodoList}"/> representing the collection of todo lists.
    /// </summary>
    public DbSet<TodoList> TodoLists => Set<TodoList>();

    /// <summary>
    /// Gets the <see cref="DbSet{TodoItem}"/> representing the collection of todo items.
    /// </summary>
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public DbSet<User> Users => throw new NotImplementedException();

    public DbSet<UserProfile> UserProfiles => throw new NotImplementedException();

    public DbSet<Session> Sessions => throw new NotImplementedException();

    public DbSet<ContactMethod> ContactMethods => throw new NotImplementedException();

    public DbSet<Address> Addresses => throw new NotImplementedException();

    public DbSet<LoginAttempt> LoginAttempts => throw new NotImplementedException();

    public DbSet<Consent> Consents => throw new NotImplementedException();

    public DbSet<UserPreference> UserPreferences => throw new NotImplementedException();

    public DbSet<Role> Roles => throw new NotImplementedException();

    public DbSet<Permission> Permissions => throw new NotImplementedException();

    public DbSet<Group> Groups => throw new NotImplementedException();

    public DbSet<UserRole> UserRoles => throw new NotImplementedException();

    public DbSet<RolePermission> RolePermissions => throw new NotImplementedException();

    public DbSet<UserGroup> UserGroups => throw new NotImplementedException();

    public DbSet<GroupRole> GroupRoles => throw new NotImplementedException();

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
    }
}
