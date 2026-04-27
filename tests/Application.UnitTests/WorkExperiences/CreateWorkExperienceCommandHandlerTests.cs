#nullable enable
using Application.UnitTests.TestInfrastructure;
using Application.WorkExperiences.Commands;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.UnitTests.WorkExperiences;

/// <summary>
/// Unit tests for <see cref="CreateWorkExperienceCommandHandler"/>.
/// </summary>
public class CreateWorkExperienceCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler creates a work experience record when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_CreatesWorkExperience_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateWorkExperienceCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new CreateWorkExperienceCommand
        {
            UserId = user.Id,
            Company = " Cerxos ",
            Position = " CTO ",
            StartDate = "2020-01-01",
            Location = " Kuala Lumpur "
        }, CancellationToken.None);

        Assert.True(result.Success);

        var workExperience = await context.WorkExperiences.SingleAsync();
        Assert.Equal(user.Id, workExperience.UserId);
        Assert.Equal("Cerxos", workExperience.Company);
        Assert.Equal("CTO", workExperience.Position);
        Assert.Equal(new LocalDate(2020, 1, 1), workExperience.StartDate);
        Assert.Null(workExperience.EndDate);
        Assert.Equal("Kuala Lumpur", workExperience.Location);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserMissing_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new CreateWorkExperienceCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new CreateWorkExperienceCommand
        {
            UserId = Guid.NewGuid(),
            Company = "Cerxos",
            Position = "Developer",
            StartDate = "2020-01-01"
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

        var handler = new CreateWorkExperienceCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new CreateWorkExperienceCommand
        {
            UserId = user.Id,
            Company = "Cerxos",
            Position = "CTO",
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
