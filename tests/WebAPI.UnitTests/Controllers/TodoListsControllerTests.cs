using Application.Common.Models;
using Application.TodoLists.Commands;
using Application.TodoLists.Models;
using Application.TodoLists.Queries;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using WebAPI.UnitTests.TestInfrastructure;

namespace WebAPI.UnitTests.Controllers;

/// <summary>
/// Unit tests for <see cref="TodoListsController"/>.
/// </summary>
public class TodoListsControllerTests
{
    [Fact]
    public async Task GetAllTodoLists_ReturnsOkResultWithResponse()
    {
        var response = BaseResponse<PaginatedEnumerable<TodoListDto>>.Ok(
            new PaginatedEnumerable<TodoListDto>(Array.Empty<TodoListDto>(), 0, 1, 10),
            "ok");

        var mediator = new TestMediator(_ => Task.FromResult<object>(response));
        var controller = new TodoListsController(mediator);

        var result = await controller.GetAllTodoLists(new GetTodoListsQuery());

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task CreateTodoList_ReturnsCreatedAtAction()
    {
        var dto = new TodoListDto(new Domain.Entities.TodoList { Id = 42, Title = "List" });
        var response = BaseResponse<TodoListDto>.Ok(dto, "created");

        var mediator = new TestMediator(_ => Task.FromResult<object>(response));
        var controller = new TodoListsController(mediator);

        var command = new CreateTodoListCommand { Title = "List" };
        var result = await controller.CreateTodoList(command);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(TodoListsController.GetAllTodoLists), created.ActionName);
        Assert.Equal(42L, created.RouteValues?["id"]);
        Assert.Same(response, created.Value);
    }

    [Fact]
    public async Task UpdateTodoList_ReturnsOkResult()
    {
        var response = BaseResponse<TodoListDto>.Ok(new TodoListDto(new Domain.Entities.TodoList()));
        var mediator = new TestMediator(_ => Task.FromResult<object>(response));
        var controller = new TodoListsController(mediator);

        var result = await controller.UpdateTodoList(new UpdateTodoListCommand { Id = 5, Title = "Updated" });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task DeleteTodoList_SendsCommandWithId()
    {
        var response = BaseResponse<string>.Ok("deleted");
        var mediator = new TestMediator(_ => Task.FromResult<object>(response));
        var controller = new TodoListsController(mediator);

        var result = await controller.DeleteTodoList(15);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, ok.Value);
        var command = Assert.IsType<DeleteTodoListCommand>(mediator.LastRequest);
        Assert.Equal(15, command.Id);
    }
}
