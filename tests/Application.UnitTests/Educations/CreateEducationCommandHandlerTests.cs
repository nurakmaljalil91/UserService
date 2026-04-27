#nullable enable
using Application.Educations.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.UnitTests.Educations;

/// <summary>
/// Unit tests for <see cref="CreateEducationCommandHandler"/>.
/// </summary>
public class CreateEducationCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler creates an education record when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_CreatesEducation_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateEducationCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new CreateEducationCommand
        {
            UserId = user.Id,
            Institution = " MIT ",
            Degree = " Master of Science ",
            FieldOfStudy = " Computer Science ",
            StartDate = "2010-09-01",
            EndDate = "2012-06-30"
        }, CancellationToken.None);

        Assert.True(result.Success);

        var education = await context.Educations.SingleAsync();
        Assert.Equal(user.Id, education.UserId);
        Assert.Equal("MIT", education.Institution);
        Assert.Equal("Master of Science", education.Degree);
        Assert.Equal("Computer Science", education.FieldOfStudy);
        Assert.Equal(new LocalDate(2010, 9, 1), education.StartDate);
        Assert.Equal(new LocalDate(2012, 6, 30), education.EndDate);
    }

    /// <summary>
    /// Ensures the handler creates an education record with null end date when end date is not provided.
    /// </summary>
    [Fact]
    public async Task Handle_CreatesEducation_WithNullEndDate_WhenEndDateNotProvided()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateEducationCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new CreateEducationCommand
        {
            UserId = user.Id,
            Institution = "MIT",
            StartDate = "2010-09-01"
        }, CancellationToken.None);

        Assert.True(result.Success);
        var education = await context.Educations.SingleAsync();
        Assert.Null(education.EndDate);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the user does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserMissing_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new CreateEducationCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new CreateEducationCommand
        {
            UserId = Guid.NewGuid(),
            Institution = "MIT",
            StartDate = "2010-09-01"
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

        var handler = new CreateEducationCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new CreateEducationCommand
        {
            UserId = user.Id,
            Institution = "MIT",
            StartDate = "01-09-2010"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Start date must be in yyyy-MM-dd format.", result.Message);
    }

    /// <summary>
    /// Ensures the handler returns a failure response when the end date is invalid.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEndDateInvalid_ReturnsFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateEducationCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new CreateEducationCommand
        {
            UserId = user.Id,
            Institution = "MIT",
            StartDate = "2010-09-01",
            EndDate = "bad-date"
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("End date must be in yyyy-MM-dd format.", result.Message);
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
