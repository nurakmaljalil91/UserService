#nullable enable
using Application.Common.Exceptions;
using Application.Educations.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.UnitTests.Educations;

/// <summary>
/// Unit tests for <see cref="DeleteEducationCommandHandler"/>.
/// </summary>
public class DeleteEducationCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler deletes an education record when the request is valid.
    /// </summary>
    [Fact]
    public async Task Handle_DeletesEducation_WhenValid()
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

        var handler = new DeleteEducationCommandHandler(context);

        var result = await handler.Handle(new DeleteEducationCommand { Id = education.Id }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Empty(await context.Educations.ToListAsync());
    }

    /// <summary>
    /// Ensures the handler throws a <see cref="NotFoundException"/> when the education record does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEducationNotFound_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new DeleteEducationCommandHandler(context);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new DeleteEducationCommand { Id = Guid.NewGuid() }, CancellationToken.None));
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
