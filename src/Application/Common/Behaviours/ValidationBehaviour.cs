using Mediator;
using FluentValidation;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.Common.Behaviours;

/// <summary>
/// Pipeline behavior that validates requests using registered <see cref="IValidator{T}"/>s.
/// Throws <see cref="ValidationException"/> if validation fails.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="validators">The validators for the request type.</param>
    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
                throw new ValidationException(failures);
        }
        return await next();
    }
}
