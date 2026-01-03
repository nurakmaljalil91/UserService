using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Security;
using Mediator;
using System.Reflection;

namespace Application.Common.Behaviours;

/// <summary>
/// Pipeline behavior that enforces authorization requirements for requests decorated with <see cref="AuthorizeAttribute"/>.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IUser _user;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="user">The current user service.</param>
    public AuthorizationBehaviour(
        IUser user)
    {
        _user = user;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

        if (authorizeAttributes.Any())
        {
            // Must be authenticated user
            if (_user.Username == null)
            {
                throw new UnauthorizedAccessException();
            }

            // Role-based authorization
            var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

            if (authorizeAttributesWithRoles.Any())
            {
                var authorized = false;

                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                {
                    foreach (var role in roles)
                    {
                        var isInRole = _user.GetRoles().Contains(role);
                        if (isInRole)
                        {
                            authorized = true;
                            break;
                        }
                    }
                }

                // Must be a member of at least one role in roles
                if (!authorized)
                {
                    throw new ForbiddenAccessException();
                }
            }

            // Policy-based authorization          
        }

        // User is authorized / authorization not required
        return await next();
    }
}
