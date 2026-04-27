#nullable enable
using Application.Common.Exceptions;
using Application.Skills.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Skills;

/// <summary>
/// Unit tests for <see cref="DeleteSkillCommandHandler"/>.
/// </summary>
public class DeleteSkillCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler deletes a skill when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_DeletesSkill_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var skill = new Skill { Id = Guid.NewGuid(), UserId = user.Id, Name = "C#" };
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        var handler = new DeleteSkillCommandHandler(context);

        var result = await handler.Handle(new DeleteSkillCommand { Id = skill.Id }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Empty(await context.Skills.ToListAsync());
    }

    /// <summary>
    /// Ensures the handler throws a <see cref="NotFoundException"/> when the skill does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenSkillNotFound_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new DeleteSkillCommandHandler(context);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new DeleteSkillCommand { Id = Guid.NewGuid() }, CancellationToken.None));
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
