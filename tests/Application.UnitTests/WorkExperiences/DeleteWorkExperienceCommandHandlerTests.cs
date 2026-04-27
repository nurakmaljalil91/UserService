#nullable enable
using Application.Common.Exceptions;
using Application.UnitTests.TestInfrastructure;
using Application.WorkExperiences.Commands;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.UnitTests.WorkExperiences;

/// <summary>
/// Unit tests for <see cref="DeleteWorkExperienceCommandHandler"/>.
/// </summary>
public class DeleteWorkExperienceCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler deletes a work experience record when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_DeletesWorkExperience_WhenValid()
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

        var handler = new DeleteWorkExperienceCommandHandler(context);

        var result = await handler.Handle(new DeleteWorkExperienceCommand { Id = workExperience.Id }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Empty(await context.WorkExperiences.ToListAsync());
    }

    /// <summary>
    /// Ensures the handler throws a <see cref="NotFoundException"/> when the record does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenWorkExperienceNotFound_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new DeleteWorkExperienceCommandHandler(context);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new DeleteWorkExperienceCommand { Id = Guid.NewGuid() }, CancellationToken.None));
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
