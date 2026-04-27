#nullable enable
using Application.Skills.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Skills;

/// <summary>
/// Unit tests for <see cref="CreateSkillCommandHandler"/>.
/// </summary>
public class CreateSkillCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler creates a skill when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_CreatesSkill_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateSkillCommandHandler(context);

        var result = await handler.Handle(new CreateSkillCommand
        {
            UserId = user.Id,
            Name = " C# ",
            Proficiency = " Expert ",
            YearsOfExperience = 5
        }, CancellationToken.None);

        Assert.True(result.Success);

        var skill = await context.Skills.SingleAsync();
        Assert.Equal(user.Id, skill.UserId);
        Assert.Equal("C#", skill.Name);
        Assert.Equal("Expert", skill.Proficiency);
        Assert.Equal(5, skill.YearsOfExperience);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserMissing_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new CreateSkillCommandHandler(context);

        var result = await handler.Handle(new CreateSkillCommand
        {
            UserId = Guid.NewGuid(),
            Name = "C#"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User does not exist.", result.Message);
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
