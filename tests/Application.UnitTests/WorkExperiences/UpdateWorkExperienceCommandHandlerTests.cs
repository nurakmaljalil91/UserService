#nullable enable
using Application.Common.Exceptions;
using Application.UnitTests.TestInfrastructure;
using Application.WorkExperiences.Commands;
using Domain.Entities;
using NodaTime;

namespace Application.UnitTests.WorkExperiences;

/// <summary>
/// Unit tests for <see cref="UpdateWorkExperienceCommandHandler"/>.
/// </summary>
public class UpdateWorkExperienceCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler updates a work experience record when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_UpdatesWorkExperience_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var workExperience = new WorkExperience
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Company = "Old Corp",
            Position = "Developer",
            StartDate = new LocalDate(2018, 1, 1)
        };
        context.WorkExperiences.Add(workExperience);
        await context.SaveChangesAsync();

        var handler = new UpdateWorkExperienceCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new UpdateWorkExperienceCommand
        {
            Id = workExperience.Id,
            Company = " New Corp ",
            Position = " Senior Developer "
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("New Corp", result.Data?.Company);
        Assert.Equal("Senior Developer", result.Data?.Position);
    }

    /// <summary>
    /// Ensures the handler throws a <see cref="NotFoundException"/> when the record does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenWorkExperienceNotFound_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new UpdateWorkExperienceCommandHandler(context, new TestClockService());

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new UpdateWorkExperienceCommand
        {
            Id = Guid.NewGuid(),
            Company = "Some Corp"
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

        var workExperience = new WorkExperience
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Company = "Cerxos",
            Position = "CTO",
            StartDate = new LocalDate(2020, 1, 1)
        };
        context.WorkExperiences.Add(workExperience);
        await context.SaveChangesAsync();

        var handler = new UpdateWorkExperienceCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new UpdateWorkExperienceCommand
        {
            Id = workExperience.Id,
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
