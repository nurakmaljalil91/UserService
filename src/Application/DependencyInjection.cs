using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Application.Authentications.Commands;
using Application.Authentications.Models;
using Application.Common.Behaviours;
using Application.Common.Models;
using Application.Permissions.Commands;
using Application.Permissions.Models;
using Application.Permissions.Queries;
using Application.Roles.Commands;
using Application.Roles.Models;
using Application.Roles.Queries;
using Application.TodoItems.Commands;
using Application.TodoItems.Models;
using Application.TodoItems.Queries;
using Application.TodoLists.Commands;
using Application.TodoLists.Models;
using Application.TodoLists.Queries;
using Application.Users.Commands;
using Application.Users.Models;
using Application.Users.Queries;
using Domain.Common;
using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

/// <summary>
/// Provides extension methods for registering application services in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers application-specific services to the provided <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddScoped<IMediator, Mediator.Mediator>();
        services.AddScoped<IRequestHandler<GetTodoItemsQuery, BaseResponse<PaginatedEnumerable<TodoItemDto>>>,
            GetTodoItemsQueryHandler>();
        services.AddScoped<IRequestHandler<CreateTodoItemCommand, BaseResponse<TodoItemDto>>, CreateTodoItemCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateTodoItemComand, BaseResponse<TodoItemDto>>, UpdateTodoItemCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteTodoItemCommand, BaseResponse<object>>, DeleteTodoItemCommandHandler>();
        services.AddScoped<IRequestHandler<LoginCommand, BaseResponse<LoginResponse>>, LoginCommandHandler>();
        services.AddScoped<IRequestHandler<RegisterCommand, BaseResponse<string>>, RegisterCommandHandler>();
        services.AddScoped<IRequestHandler<ResetPasswordCommand, BaseResponse<string>>, ResetPasswordCommandHandler>();
        services.AddScoped<IRequestHandler<CreateUserCommand, BaseResponse<UserDto>>, CreateUserCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateUserCommand, BaseResponse<UserDto>>, UpdateUserCommandHandler>();
        services.AddScoped<IRequestHandler<SoftDeleteUserCommand, BaseResponse<string>>, SoftDeleteUserCommandHandler>();
        services.AddScoped<IRequestHandler<GetUserByIdQuery, BaseResponse<UserDto>>, GetUserByIdQueryHandler>();
        services.AddScoped<IRequestHandler<GetUsersQuery, BaseResponse<PaginatedEnumerable<UserDto>>>, GetUsersQueryHandler>();
        services.AddScoped<IRequestHandler<GetRolesQuery, BaseResponse<PaginatedEnumerable<RoleDto>>>, GetRolesQueryHandler>();
        services.AddScoped<IRequestHandler<GetRoleByIdQuery, BaseResponse<RoleDto>>, GetRoleByIdQueryHandler>();
        services.AddScoped<IRequestHandler<CreateRoleCommand, BaseResponse<RoleDto>>, CreateRoleCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateRoleCommand, BaseResponse<RoleDto>>, UpdateRoleCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteRoleCommand, BaseResponse<string>>, DeleteRoleCommandHandler>();
        services.AddScoped<IRequestHandler<GetPermissionsQuery, BaseResponse<PaginatedEnumerable<PermissionDto>>>,
            GetPermissionsQueryHandler>();
        services.AddScoped<IRequestHandler<GetPermissionByIdQuery, BaseResponse<PermissionDto>>, GetPermissionByIdQueryHandler>();
        services.AddScoped<IRequestHandler<CreatePermissionCommand, BaseResponse<PermissionDto>>, CreatePermissionCommandHandler>();
        services.AddScoped<IRequestHandler<UpdatePermissionCommand, BaseResponse<PermissionDto>>, UpdatePermissionCommandHandler>();
        services.AddScoped<IRequestHandler<DeletePermissionCommand, BaseResponse<string>>, DeletePermissionCommandHandler>();
        services.AddScoped<IRequestHandler<GetTodoListsQuery, BaseResponse<PaginatedEnumerable<TodoListDto>>>,
            GetTodoListsQueryHandler>();
        services.AddScoped<IRequestHandler<CreateTodoListCommand, BaseResponse<TodoListDto>>, CreateTodoListCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateTodoListCommand, BaseResponse<TodoListDto>>, UpdateTodoListCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteTodoListCommand, BaseResponse<string>>, DeleteTodoListCommandHandler>();
        return services;
    }
}
