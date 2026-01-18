using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Application.Authentications.Commands;
using Application.Authentications.Models;
using Application.Common.Behaviours;
using Application.Common.Models;
using Application.ExternalLinks.Commands;
using Application.ExternalLinks.Models;
using Application.ExternalLinks.Queries;
using Application.Groups.Commands;
using Application.Groups.Models;
using Application.Groups.Queries;
using Application.Addresses.Commands;
using Application.Addresses.Models;
using Application.Addresses.Queries;
using Application.Consents.Commands;
using Application.Consents.Models;
using Application.Consents.Queries;
using Application.ContactMethods.Commands;
using Application.ContactMethods.Models;
using Application.ContactMethods.Queries;
using Application.Permissions.Commands;
using Application.Permissions.Models;
using Application.Permissions.Queries;
using Application.Roles.Commands;
using Application.Roles.Models;
using Application.Roles.Queries;
using Application.Users.Commands;
using Application.Users.Models;
using Application.Users.Queries;
using Application.UserProfiles.Commands;
using Application.UserProfiles.Models;
using Application.UserProfiles.Queries;
using Application.UserPreferences.Commands;
using Application.UserPreferences.Models;
using Application.UserPreferences.Queries;
using Application.Sessions.Commands;
using Application.Sessions.Models;
using Application.Sessions.Queries;
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
        services.AddScoped<IRequestHandler<LoginCommand, BaseResponse<LoginResponse>>, LoginCommandHandler>();
        services.AddScoped<IRequestHandler<RefreshTokenCommand, BaseResponse<LoginResponse>>, RefreshTokenCommandHandler>();
        services.AddScoped<IRequestHandler<RegisterCommand, BaseResponse<string>>, RegisterCommandHandler>();
        services.AddScoped<IRequestHandler<ResetPasswordCommand, BaseResponse<string>>, ResetPasswordCommandHandler>();
        services.AddScoped<IRequestHandler<CreateUserCommand, BaseResponse<UserDto>>, CreateUserCommandHandler>();
        services.AddScoped<IRequestHandler<AssignRoleToUserCommand, BaseResponse<UserDto>>, AssignRoleToUserCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateUserCommand, BaseResponse<UserDto>>, UpdateUserCommandHandler>();
        services.AddScoped<IRequestHandler<SoftDeleteUserCommand, BaseResponse<string>>, SoftDeleteUserCommandHandler>();
        services.AddScoped<IRequestHandler<GetUserByIdQuery, BaseResponse<UserDto>>, GetUserByIdQueryHandler>();
        services.AddScoped<IRequestHandler<GetUsersQuery, BaseResponse<PaginatedEnumerable<UserDto>>>, GetUsersQueryHandler>();
        services.AddScoped<IRequestHandler<GetAddressesQuery, BaseResponse<PaginatedEnumerable<AddressDto>>>,
            GetAddressesQueryHandler>();
        services.AddScoped<IRequestHandler<GetAddressByIdQuery, BaseResponse<AddressDto>>, GetAddressByIdQueryHandler>();
        services.AddScoped<IRequestHandler<CreateAddressCommand, BaseResponse<AddressDto>>, CreateAddressCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateAddressCommand, BaseResponse<AddressDto>>, UpdateAddressCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteAddressCommand, BaseResponse<string>>, DeleteAddressCommandHandler>();
        services.AddScoped<IRequestHandler<GetConsentsQuery, BaseResponse<PaginatedEnumerable<ConsentDto>>>,
            GetConsentsQueryHandler>();
        services.AddScoped<IRequestHandler<GetConsentByIdQuery, BaseResponse<ConsentDto>>, GetConsentByIdQueryHandler>();
        services.AddScoped<IRequestHandler<CreateConsentCommand, BaseResponse<ConsentDto>>, CreateConsentCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateConsentCommand, BaseResponse<ConsentDto>>, UpdateConsentCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteConsentCommand, BaseResponse<string>>, DeleteConsentCommandHandler>();
        services.AddScoped<IRequestHandler<GetContactMethodsQuery, BaseResponse<PaginatedEnumerable<ContactMethodDto>>>,
            GetContactMethodsQueryHandler>();
        services.AddScoped<IRequestHandler<GetContactMethodByIdQuery, BaseResponse<ContactMethodDto>>, GetContactMethodByIdQueryHandler>();
        services.AddScoped<IRequestHandler<CreateContactMethodCommand, BaseResponse<ContactMethodDto>>, CreateContactMethodCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateContactMethodCommand, BaseResponse<ContactMethodDto>>, UpdateContactMethodCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteContactMethodCommand, BaseResponse<string>>, DeleteContactMethodCommandHandler>();
        services.AddScoped<IRequestHandler<GetUserProfilesQuery, BaseResponse<PaginatedEnumerable<UserProfileDto>>>,
            GetUserProfilesQueryHandler>();
        services.AddScoped<IRequestHandler<GetUserProfileByIdQuery, BaseResponse<UserProfileDto>>, GetUserProfileByIdQueryHandler>();
        services.AddScoped<IRequestHandler<CreateUserProfileCommand, BaseResponse<UserProfileDto>>, CreateUserProfileCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateUserProfileCommand, BaseResponse<UserProfileDto>>, UpdateUserProfileCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteUserProfileCommand, BaseResponse<string>>, DeleteUserProfileCommandHandler>();
        services.AddScoped<IRequestHandler<GetUserPreferencesQuery, BaseResponse<PaginatedEnumerable<UserPreferenceDto>>>,
            GetUserPreferencesQueryHandler>();
        services.AddScoped<IRequestHandler<GetUserPreferenceByIdQuery, BaseResponse<UserPreferenceDto>>, GetUserPreferenceByIdQueryHandler>();
        services.AddScoped<IRequestHandler<CreateUserPreferenceCommand, BaseResponse<UserPreferenceDto>>, CreateUserPreferenceCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateUserPreferenceCommand, BaseResponse<UserPreferenceDto>>, UpdateUserPreferenceCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteUserPreferenceCommand, BaseResponse<string>>, DeleteUserPreferenceCommandHandler>();
        services.AddScoped<IRequestHandler<GetSessionsQuery, BaseResponse<PaginatedEnumerable<SessionDto>>>,
            GetSessionsQueryHandler>();
        services.AddScoped<IRequestHandler<GetSessionByIdQuery, BaseResponse<SessionDto>>, GetSessionByIdQueryHandler>();
        services.AddScoped<IRequestHandler<CreateSessionCommand, BaseResponse<SessionDto>>, CreateSessionCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateSessionCommand, BaseResponse<SessionDto>>, UpdateSessionCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteSessionCommand, BaseResponse<string>>, DeleteSessionCommandHandler>();
        services.AddScoped<IRequestHandler<GetRolesQuery, BaseResponse<PaginatedEnumerable<RoleDto>>>, GetRolesQueryHandler>();
        services.AddScoped<IRequestHandler<GetRoleByIdQuery, BaseResponse<RoleDto>>, GetRoleByIdQueryHandler>();
        services.AddScoped<IRequestHandler<CreateRoleCommand, BaseResponse<RoleDto>>, CreateRoleCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateRoleCommand, BaseResponse<RoleDto>>, UpdateRoleCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteRoleCommand, BaseResponse<string>>, DeleteRoleCommandHandler>();
        services.AddScoped<IRequestHandler<AssignPermissionToRoleCommand, BaseResponse<RoleDto>>, AssignPermissionToRoleCommandHandler>();
        services.AddScoped<IRequestHandler<GetPermissionsQuery, BaseResponse<PaginatedEnumerable<PermissionDto>>>,
            GetPermissionsQueryHandler>();
        services.AddScoped<IRequestHandler<GetPermissionByIdQuery, BaseResponse<PermissionDto>>, GetPermissionByIdQueryHandler>();
        services.AddScoped<IRequestHandler<CreatePermissionCommand, BaseResponse<PermissionDto>>, CreatePermissionCommandHandler>();
        services.AddScoped<IRequestHandler<UpdatePermissionCommand, BaseResponse<PermissionDto>>, UpdatePermissionCommandHandler>();
        services.AddScoped<IRequestHandler<DeletePermissionCommand, BaseResponse<string>>, DeletePermissionCommandHandler>();
        services.AddScoped<IRequestHandler<GetGroupsQuery, BaseResponse<PaginatedEnumerable<GroupDto>>>, GetGroupsQueryHandler>();
        services.AddScoped<IRequestHandler<GetGroupByIdQuery, BaseResponse<GroupDto>>, GetGroupByIdQueryHandler>();
        services.AddScoped<IRequestHandler<CreateGroupCommand, BaseResponse<GroupDto>>, CreateGroupCommandHandler>();
        services.AddScoped<IRequestHandler<AssignRoleToGroupCommand, BaseResponse<GroupDto>>, AssignRoleToGroupCommandHandler>();
        services.AddScoped<IRequestHandler<AssignUserToGroupCommand, BaseResponse<GroupDto>>, AssignUserToGroupCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateGroupCommand, BaseResponse<GroupDto>>, UpdateGroupCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteGroupCommand, BaseResponse<string>>, DeleteGroupCommandHandler>();
        services.AddScoped<IRequestHandler<StartExternalLinkCommand, BaseResponse<ExternalLinkStartResponse>>,
            StartExternalLinkCommandHandler>();
        services.AddScoped<IRequestHandler<CompleteExternalLinkCommand, BaseResponse<ExternalLinkDto>>,
            CompleteExternalLinkCommandHandler>();
        services.AddScoped<IRequestHandler<UnlinkExternalProviderCommand, BaseResponse<string>>,
            UnlinkExternalProviderCommandHandler>();
        services.AddScoped<IRequestHandler<GetExternalLinksQuery, BaseResponse<IReadOnlyCollection<ExternalLinkDto>>>,
            GetExternalLinksQueryHandler>();
        services.AddScoped<IRequestHandler<GetGoogleCalendarAccessTokenQuery, BaseResponse<ExternalAccessTokenDto>>,
            GetGoogleCalendarAccessTokenQueryHandler>();
        return services;
    }
}
