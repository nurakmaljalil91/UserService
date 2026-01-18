#nullable enable
using Application.Groups.Commands;
using Application.UnitTests.TestInfrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Groups;

/// <summary>
/// Unit tests for <see cref="CreateGroupCommandHandler"/>.
/// </summary>
public class CreateGroupCommandHandlerTests
{
    /// <summary>
    /// Ensures the handler creates a group and normalizes the name.
    /// </summary>
    [Fact]
    public async Task Handle_CreatesGroup_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new CreateGroupCommandHandler(context);

        var result = await handler.Handle(new CreateGroupCommand
        {
            Name = " staff ",
            Description = " Staff group "
        }, CancellationToken.None);

        Assert.True(result.Success);

        var group = await context.Groups.SingleAsync();
        Assert.Equal("staff", group.Name);
        Assert.Equal("STAFF", group.NormalizedName);
        Assert.Equal("Staff group", group.Description);
    }
}
