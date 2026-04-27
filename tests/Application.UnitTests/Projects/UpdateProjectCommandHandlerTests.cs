#nullable enable
using Application.Common.Exceptions;
using Application.Projects.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;

namespace Application.UnitTests.Projects;

/// <summary>
/// Unit tests for <see cref="UpdateProjectCommandHandler"/>.
/// </summary>
public class UpdateProjectCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler updates a project when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_UpdatesProject_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var project = new Project { Id = Guid.NewGuid(), UserId = user.Id, Title = "Old Title" };
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var handler = new UpdateProjectCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new UpdateProjectCommand
        {
            Id = project.Id,
            Title = " New Title ",
            TechStack = " .NET, Angular "
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("New Title", result.Data?.Title);
        Assert.Equal(".NET, Angular", result.Data?.TechStack);
    }

    /// <summary>
    /// Ensures the handler throws a <see cref="NotFoundException"/> when the project does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenProjectNotFound_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new UpdateProjectCommandHandler(context, new TestClockService());

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new UpdateProjectCommand
        {
            Id = Guid.NewGuid(),
            Title = "Some Project"
        }, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the start date format is invalid.
    /// </summary>
    [Fact]
    public async Task Handle_WhenStartDateInvalid_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var project = new Project { Id = Guid.NewGuid(), UserId = user.Id, Title = "My Project" };
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var handler = new UpdateProjectCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new UpdateProjectCommand
        {
            Id = project.Id,
            StartDate = "bad-date"
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
