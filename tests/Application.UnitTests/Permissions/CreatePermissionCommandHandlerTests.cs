#nullable enable
using Application.Permissions.Commands;
using Application.UnitTests.TestInfrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Permissions;

/// <summary>
/// Unit tests for <see cref="CreatePermissionCommandHandler"/>.
/// </summary>
public class CreatePermissionCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler creates a permission and normalizes the name.
    /// </summary>
    [Fact]
    public async Task Handle_CreatesPermission_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new CreatePermissionCommandHandler(context);

        var result = await handler.Handle(new CreatePermissionCommand
        {
            Name = " user.read ",
            Description = " Read access "
        }, CancellationToken.None);

        Assert.True(result.Success);

        var permission = await context.Permissions.SingleAsync();
        Assert.Equal("user.read", permission.Name);
        Assert.Equal("USER.READ", permission.NormalizedName);
        Assert.Equal("Read access", permission.Description);
    }
}
