#nullable enable
using Application.Users.Commands;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Users;

/// <summary>
/// Contains unit tests for <see cref="SoftDeleteUserCommandHandler"/>.
/// </summary>
public class SoftDeleteUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_SetsIsDeleted_WhenFound()
    {
        await using var context = TestDbContextFactory.Create();
        var user = new User
        {
            Username = "user",
            NormalizedUsername = "USER",
            Email = "user@example.com",
            NormalizedEmail = "USER@EXAMPLE.COM",
            IsDeleted = false
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new SoftDeleteUserCommandHandler(context);

        var result = await handler.Handle(new SoftDeleteUserCommand
        {
            Id = user.Id
        }, CancellationToken.None);

        Assert.True(result.Success);

        var deleted = await context.Users.SingleAsync();
        Assert.True(deleted.IsDeleted);
    }
}
