#nullable enable
using Application.Permissions.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Permissions;

/// <summary>
/// Unit tests for <see cref="GetPermissionsQueryHandler"/>.
/// </summary>
public class GetPermissionsQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler returns a paginated list of permissions.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsPaginatedPermissions()
    {
        await using var context = TestDbContextFactory.Create();
        context.Permissions.AddRange(
            new Permission { Name = "read", NormalizedName = "READ" },
            new Permission { Name = "write", NormalizedName = "WRITE" });
        await context.SaveChangesAsync();

        var handler = new GetPermissionsQueryHandler(context);

        var result = await handler.Handle(new GetPermissionsQuery
        {
            Page = 1,
            Total = 10
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.TotalCount);
        Assert.Equal(2, await context.Permissions.CountAsync());
    }
}
