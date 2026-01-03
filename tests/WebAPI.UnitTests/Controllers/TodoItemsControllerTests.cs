using Application.Common.Models;
using Application.TodoItems.Commands;
using Application.TodoItems.Models;
using Application.TodoItems.Queries;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using WebAPI.UnitTests.TestInfrastructure;

namespace WebAPI.UnitTests.Controllers;

/// <summary>
/// Unit tests for <see cref="TodoItemsController"/>.
/// </summary>
public class TodoItemsControllerTests
{
    [Fact]
    public async Task GetTodoItems_ReturnsOkResultWithResponse()
    {
        var response = BaseResponse<PaginatedEnumerable<TodoItemDto>>.Ok(
            new PaginatedEnumerable<TodoItemDto>(Array.Empty<TodoItemDto>(), 0, 1, 10),
            "ok");

        var mediator = new TestMediator(_ => Task.FromResult<object>(response));
        var controller = new TodoItemsController(mediator);

        var result = await controller.GetTodoItems(new GetTodoItemsQuery());

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task CreateTodoItem_ReturnsCreatedAtAction()
    {
        var dto = new TodoItemDto(new Domain.Entities.TodoItem { Id = 9, Title = "Item" });
        var response = BaseResponse<TodoItemDto>.Ok(dto, "created");

        var mediator = new TestMediator(_ => Task.FromResult<object>(response));
        var controller = new TodoItemsController(mediator);

        var result = await controller.CreateTodoItem(new CreateTodoItemCommand { Title = "Item" });

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(TodoItemsController.GetTodoItems), created.ActionName);
        Assert.Equal(9L, created.RouteValues?["id"]);
        Assert.Same(response, created.Value);
    }

    [Fact]
    public async Task UpdateTodoItem_ReturnsOkResult()
    {
        var response = BaseResponse<TodoItemDto>.Ok(new TodoItemDto(new Domain.Entities.TodoItem()));
        var mediator = new TestMediator(_ => Task.FromResult<object>(response));
        var controller = new TodoItemsController(mediator);

        var result = await controller.UpdateTodoItem(new UpdateTodoItemComand { Id = 5, Title = "Updated" });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task DeleteTodoItem_SendsCommandWithId()
    {
        var response = BaseResponse<object>.Ok(null, "deleted");
        var mediator = new TestMediator(_ => Task.FromResult<object>(response));
        var controller = new TodoItemsController(mediator);

        var result = await controller.DeleteTodoItem(18);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, ok.Value);
        var command = Assert.IsType<DeleteTodoItemCommand>(mediator.LastRequest);
        Assert.Equal(18, command.Id);
    }
}
