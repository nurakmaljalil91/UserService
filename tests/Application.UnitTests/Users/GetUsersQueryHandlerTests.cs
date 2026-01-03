#nullable enable
using System.Linq;
using Application.Users.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;

namespace Application.UnitTests.Users;

/// <summary>
/// Contains unit tests for <see cref="GetUsersQueryHandler"/>.
/// </summary>
public class GetUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ExcludesDeletedUsers()
    {
        await using var context = TestDbContextFactory.Create();
        context.Users.AddRange(
            new User
            {
                Username = "active",
                NormalizedUsername = "ACTIVE",
                Email = "active@example.com",
                NormalizedEmail = "ACTIVE@EXAMPLE.COM",
                IsDeleted = false
            },
            new User
            {
                Username = "deleted",
                NormalizedUsername = "DELETED",
                Email = "deleted@example.com",
                NormalizedEmail = "DELETED@EXAMPLE.COM",
                IsDeleted = true
            });
        await context.SaveChangesAsync();

        var handler = new GetUsersQueryHandler(context);

        var result = await handler.Handle(new GetUsersQuery { Page = 1, Total = 10 }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Single(result.Data!.Items!);
        Assert.Equal("active", result.Data!.Items!.First().Username);
    }
}
