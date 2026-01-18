using Application.Common.Interfaces;
using Domain.Common;
using Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Text;
using NodaTime.TimeZones;

namespace Infrastructure.UnitTests.Data;

/// <summary>
/// Unit tests for <see cref="AuditableEntityInterceptor"/>.
/// </summary>
public class AuditableEntityInterceptorTests
{
    /// <summary>
    /// Ensures audit fields are set for newly added entities.
    /// </summary>
    [Fact]
    public void UpdateEntities_SetsAuditFieldsForAddedEntities()
    {
        var now = Instant.FromUtc(2024, 1, 2, 3, 4, 5);
        var interceptor = new AuditableEntityInterceptor(new TestUser("alice"), new TestClockService(now));

        using var context = CreateContext();
        var entity = new TestAuditableEntity { Title = "New", Details = new OwnedDetails { Note = "note" } };

        context.Entities.Add(entity);
        interceptor.UpdateEntities(context);

        Assert.Equal("alice", entity.CreatedBy);
        Assert.Equal(now, entity.CreatedDate);
        Assert.Equal("alice", entity.UpdatedBy);
        Assert.Equal(now, entity.UpdatedDate);
    }

    /// <summary>
    /// Ensures only update fields are changed for modified entities.
    /// </summary>
    [Fact]
    public void UpdateEntities_DoesNotOverwriteCreatedFieldsForModifiedEntities()
    {
        var created = Instant.FromUtc(2023, 12, 1, 10, 0, 0);
        var now = Instant.FromUtc(2024, 1, 2, 3, 4, 5);
        var interceptor = new AuditableEntityInterceptor(new TestUser("alice"), new TestClockService(now));

        using var context = CreateContext();
        var entity = new TestAuditableEntity
        {
            Title = "Existing",
            CreatedBy = "system",
            CreatedDate = created,
            Details = new OwnedDetails { Note = "note" }
        };

        context.Entities.Attach(entity);
        context.Entry(entity).State = EntityState.Modified;

        interceptor.UpdateEntities(context);

        Assert.Equal("system", entity.CreatedBy);
        Assert.Equal(created, entity.CreatedDate);
        Assert.Equal("alice", entity.UpdatedBy);
        Assert.Equal(now, entity.UpdatedDate);
    }

    /// <summary>
    /// Ensures owned entity changes are detected by the change tracker.
    /// </summary>
    [Fact]
    public void HasChangedOwnedEntities_ReturnsTrueWhenOwnedEntityIsModified()
    {
        using var context = CreateContext();
        var entity = new TestAuditableEntity { Title = "Owner", Details = new OwnedDetails { Note = "initial" } };

        context.Entities.Attach(entity);

        var entry = context.Entry(entity);
        entry.State = EntityState.Unchanged;
        entry.Reference(e => e.Details).TargetEntry!.State = EntityState.Modified;

        Assert.True(entry.HasChangedOwnedEntities());
    }

    private static TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestDbContext(options);
    }

#nullable enable
    private sealed class TestUser : IUser
    {
        public TestUser(string username)
        {
            Username = username;
        }

        /// <summary>
        /// Gets the user identifier for the test user.
        /// </summary>
        public Guid? UserId { get; }

        public string? Username { get; }

        public List<string> GetRoles() => new();
    }
#nullable restore

    private sealed class TestClockService : IClockService
    {
        public TestClockService(Instant now)
        {
            Now = now;
            TimeZone = DateTimeZone.Utc;
        }

        public DateTimeZone TimeZone { get; }

        public Instant Now { get; }

        public LocalDateTime LocalNow => Now.InZone(TimeZone).LocalDateTime;

        public LocalDate Today => Now.InZone(TimeZone).Date;

        public Instant ToInstant(LocalDateTime local)
            => local.InZone(TimeZone, Resolvers.LenientResolver).ToInstant();

        public LocalDateTime ToLocal(Instant instant)
            => instant.InZone(TimeZone).LocalDateTime;

        public ParseResult<LocalDate>? TryParseDate(string? date)
        {
            var pattern = LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
            return date == null ? null : pattern.Parse(date);
        }

        public ParseResult<LocalDateTime>? TryParseDateTime(string? time)
        {
            var pattern = LocalDateTimePattern.CreateWithInvariantCulture("yyyy-MM-ddTHH:mm:ss");
            return time == null ? null : pattern.Parse(time);
        }
    }

    private sealed class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        public DbSet<TestAuditableEntity> Entities => Set<TestAuditableEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestAuditableEntity>()
                .OwnsOne(e => e.Details);
        }
    }

    private sealed class TestAuditableEntity : BaseEntity<long>
    {
        public string? Title { get; set; }

        public OwnedDetails? Details { get; set; }
    }

    private sealed class OwnedDetails
    {
        public string? Note { get; set; }
    }
}
