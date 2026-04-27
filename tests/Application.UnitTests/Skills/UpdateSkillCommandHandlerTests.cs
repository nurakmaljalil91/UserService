#nullable enable
using Application.Common.Exceptions;
using Application.Skills.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;

namespace Application.UnitTests.Skills;

/// <summary>
/// Unit tests for <see cref="UpdateSkillCommandHandler"/>.
/// </summary>
public class UpdateSkillCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler updates a skill when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_UpdatesSkill_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var skill = new Skill { Id = Guid.NewGuid(), UserId = user.Id, Name = "Old Skill" };
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        var handler = new UpdateSkillCommandHandler(context);

        var result = await handler.Handle(new UpdateSkillCommand
        {
            Id = skill.Id,
            Name = " New Skill ",
            Proficiency = " Intermediate ",
            YearsOfExperience = 3
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("New Skill", result.Data?.Name);
        Assert.Equal("Intermediate", result.Data?.Proficiency);
        Assert.Equal(3, result.Data?.YearsOfExperience);
    }

    /// <summary>
    /// Ensures the handler throws a <see cref="NotFoundException"/> when the skill does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenSkillNotFound_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new UpdateSkillCommandHandler(context);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new UpdateSkillCommand
        {
            Id = Guid.NewGuid(),
            Name = "Some Skill"
        }, CancellationToken.None));
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
