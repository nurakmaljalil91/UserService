using Application.Authentications.Commands;
using Application.Authentications.Models;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using WebAPI.UnitTests.TestInfrastructure;

namespace WebAPI.UnitTests.Controllers;

/// <summary>
/// Unit tests for <see cref="AuthenticationsController"/>.
/// </summary>
public class AuthenticationsControllerTests
{
    [Fact]
    public async Task Login_ReturnsOk_WhenSuccess()
    {
        var response = BaseResponse<LoginResponse>.Ok(
            new LoginResponse("token", DateTime.UtcNow, "refresh-token", DateTime.UtcNow.AddDays(30)),
            "Login successful.");
        var mediator = new TestMediator(_ => Task.FromResult<object>(response));
        var controller = new AuthenticationsController(mediator);

        var result = await controller.Login(new LoginCommand { Username = "user", Password = "pass" });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<BaseResponse<LoginResponse>>(ok.Value);
        Assert.True(payload.Success);
        Assert.Same(response, payload);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenFailure()
    {
        var response = BaseResponse<LoginResponse>.Fail("Invalid credentials.");
        var mediator = new TestMediator(_ => Task.FromResult<object>(response));
        var controller = new AuthenticationsController(mediator);

        var result = await controller.Login(new LoginCommand { Username = "user", Password = "pass" });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var payload = Assert.IsType<BaseResponse<LoginResponse>>(badRequest.Value);
        Assert.False(payload.Success);
    }

    [Fact]
    public async Task Register_ReturnsOk_WhenSuccess()
    {
        var response = BaseResponse<string>.Ok("user-id", "User registered.");
        var mediator = new TestMediator(_ => Task.FromResult<object>(response));
        var controller = new AuthenticationsController(mediator);

        var result = await controller.Register(new RegisterCommand { Username = "user", Email = "user@example.com", Password = "pass" });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
        Assert.True(payload.Success);
        Assert.Same(response, payload);
    }

    [Fact]
    public async Task Refresh_ReturnsOk_WhenSuccess()
    {
        var response = BaseResponse<LoginResponse>.Ok(
            new LoginResponse("token", DateTime.UtcNow, "refresh-token", DateTime.UtcNow.AddDays(30)),
            "Token refreshed.");
        var mediator = new TestMediator(_ => Task.FromResult<object>(response));
        var controller = new AuthenticationsController(mediator);

        var result = await controller.Refresh(new RefreshTokenCommand { RefreshToken = "refresh-token" });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<BaseResponse<LoginResponse>>(ok.Value);
        Assert.True(payload.Success);
        Assert.Same(response, payload);
    }

    [Fact]
    public async Task Refresh_ReturnsBadRequest_WhenFailure()
    {
        var response = BaseResponse<LoginResponse>.Fail("Invalid refresh token.");
        var mediator = new TestMediator(_ => Task.FromResult<object>(response));
        var controller = new AuthenticationsController(mediator);

        var result = await controller.Refresh(new RefreshTokenCommand { RefreshToken = "refresh-token" });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var payload = Assert.IsType<BaseResponse<LoginResponse>>(badRequest.Value);
        Assert.False(payload.Success);
    }
}
