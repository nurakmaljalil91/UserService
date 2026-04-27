#nullable enable
using Application.Common.Exceptions;
using Application.Educations.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using NodaTime;

namespace Application.UnitTests.Educations;

/// <summary>
/// Unit tests for <see cref="UpdateEducationCommandHandler"/>.
/// </summary>
public class UpdateEducationCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler updates an education record when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_UpdatesEducation_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var education = new Education
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Institution = "Old University",
            StartDate = new LocalDate(2010, 1, 1)
        };
        context.Educations.Add(education);
        await context.SaveChangesAsync();

        var handler = new UpdateEducationCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new UpdateEducationCommand
        {
            Id = education.Id,
            Institution = " New University ",
            Degree = " BSc "
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("New University", result.Data?.Institution);
        Assert.Equal("BSc", result.Data?.Degree);
    }

    /// <summary>
    /// Ensures the handler throws a <see cref="NotFoundException"/> when the education record does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEducationNotFound_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new UpdateEducationCommandHandler(context, new TestClockService());

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new UpdateEducationCommand
        {
            Id = Guid.NewGuid(),
            Institution = "Some University"
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

        var education = new Education
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Institution = "MIT",
            StartDate = new LocalDate(2010, 1, 1)
        };
        context.Educations.Add(education);
        await context.SaveChangesAsync();

        var handler = new UpdateEducationCommandHandler(context, new TestClockService());

        var result = await handler.Handle(new UpdateEducationCommand
        {
            Id = education.Id,
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
