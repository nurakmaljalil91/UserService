#nullable enable
using Application.Common.Exceptions;
using Application.Permissions.Queries;
using Application.UnitTests.TestInfrastructure;
using Domain.Entities;

namespace Application.UnitTests.Permissions;

/// <summary>
/// Unit tests for <see cref="GetPermissionByIdQueryHandler"/>.
/// </summary>
public class GetPermissionByIdQueryHandlerTests
{
    /// <summary>
    /// Ensures the handler throws when the permission does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenPermissionMissing_ThrowsNotFoundException()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new GetPermissionByIdQueryHandler(context);

        var query = new GetPermissionByIdQuery
        {
            Id = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(query, CancellationToken.None));
    }

    /// <summary>
    /// Ensures the handler returns the permission when it exists.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsPermission_WhenExists()
    {
        await using var context = TestDbContextFactory.Create();
        var permission = new Permission { Name = "read", NormalizedName = "READ" };
        context.Permissions.Add(permission);
        await context.SaveChangesAsync();

        var handler = new GetPermissionByIdQueryHandler(context);

        var result = await handler.Handle(new GetPermissionByIdQuery
        {
            Id = permission.Id
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(permission.Id, result.Data!.Id);
        Assert.Equal("read", result.Data!.Name);
    }
}
