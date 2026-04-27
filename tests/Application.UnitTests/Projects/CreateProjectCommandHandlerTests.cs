#nullable enable
using Application.Projects.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.UnitTests.Projects;

/// <summary>
/// Unit tests for <see cref="CreateProjectCommandHandler"/>.
/// </summary>
public class CreateProjectCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler creates a project when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_CreatesProject_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateProjectCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new CreateProjectCommand
        {
            UserId = user.Id,
            Title = " CerxosWebSystem ",
            TechStack = " .NET, Angular ",
            StartDate = "2023-01-01"
        }, CancellationToken.None);

        Assert.True(result.Success);

        var project = await context.Projects.SingleAsync();
        Assert.Equal(user.Id, project.UserId);
        Assert.Equal("CerxosWebSystem", project.Title);
        Assert.Equal(".NET, Angular", project.TechStack);
        Assert.Equal(new LocalDate(2023, 1, 1), project.StartDate);
        Assert.Null(project.EndDate);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserMissing_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new CreateProjectCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new CreateProjectCommand
        {
            UserId = Guid.NewGuid(),
            Title = "My Project"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User does not exist.", result.Message);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the start date is invalid.
    /// </summary>
    [Fact]
    public async Task Handle_WhenStartDateInvalid_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateProjectCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new CreateProjectCommand
        {
            UserId = user.Id,
            Title = "My Project",
            StartDate = "not-a-date"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Start date must be in yyyy-MM-dd format.", result.Message);
    }

    private static User CreateUser()
    {
        var unique = Guid.NewGuid().ToString("N");
        return new User
        {
            Username = $"user-{unique}",
            NormalizedUsername = $"USER-{unique}".ToUpperInvariant(),
            Email = $"user-{unique}@example.com",
            NormalizedEmail = $"USER-{unique}@EXAMPLE.COM",
            PasswordHash = "hashed",
            EmailConfirm = false,
            PhoneNumberConfirm = false,
            TwoFactorEnabled = false,
            AccessFailedCount = 0,
            IsLocked = false,
            IsDeleted = false
        };
    }
}
