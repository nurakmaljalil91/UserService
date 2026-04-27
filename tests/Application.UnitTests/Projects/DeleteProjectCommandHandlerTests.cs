#nullable enable
using Application.Common.Exceptions;
using Application.Projects.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Projects;

/// <summary>
/// Unit tests for <see cref="DeleteProjectCommandHandler"/>.
/// </summary>
public class DeleteProjectCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler deletes a project when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_DeletesProject_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var project = new Project { Id = Guid.NewGuid(), UserId = user.Id, Title = "My Project" };
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var handler = new DeleteProjectCommandHandler(context);

        var result = await handler.Handle(new DeleteProjectCommand { Id = project.Id }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Empty(await context.Projects.ToListAsync());
    }

    /// <summary>
    /// Ensures the handler throws a <see cref="NotFoundException"/> when the project does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenProjectNotFound_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new DeleteProjectCommandHandler(context);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new DeleteProjectCommand { Id = Guid.NewGuid() }, CancellationToken.None));
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
