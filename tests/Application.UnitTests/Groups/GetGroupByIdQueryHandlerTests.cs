#nullable enable
using Application.Common.Exceptions;
using Application.Groups.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;

namespace Application.UnitTests.Groups;

/// <summary>
/// Unit tests for <see cref="GetGroupByIdQueryHandler"/>.
/// </summary>
public class GetGroupByIdQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the group does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenGroupMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new GetGroupByIdQueryHandler(context);

        var query = new GetGroupByIdQuery
        {
            Id = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(query, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns the group when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsGroup_WhenExists()
    {
        await using var context = TestDbContextFactory.Create();
        var group = new Group { Name = "staff", NormalizedName = "STAFF" };
        context.Groups.Add(group);
        await context.SaveChangesAsync();

        var handler = new GetGroupByIdQueryHandler(context);

        var result = await handler.Handle(new GetGroupByIdQuery
        {
            Id = group.Id
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(group.Id, result.Data!.Id);
        Assert.Equal("staff", result.Data!.Name);
    }
}
