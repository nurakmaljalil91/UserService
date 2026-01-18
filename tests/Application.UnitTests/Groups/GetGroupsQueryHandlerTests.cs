#nullable enable
using Application.Groups.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Groups;

/// <summary>
/// Unit tests for <see cref="GetGroupsQueryHandler"/>.
/// </summary>
public class GetGroupsQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler returns a paginated list of groups.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsPaginatedGroups()
    {
        await using var context = TestDbContextFactory.Create();
        context.Groups.AddRange(
            new Group { Name = "staff", NormalizedName = "STAFF" },
            new Group { Name = "admin", NormalizedName = "ADMIN" });
        await context.SaveChangesAsync();

        var handler = new GetGroupsQueryHandler(context);

        var result = await handler.Handle(new GetGroupsQuery
        {
            Page = 1,
            Total = 10
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.TotalCount);
        Assert.Equal(2, await context.Groups.CountAsync());
    }
}
