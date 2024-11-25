using MediatorForge.CQRS.Commands;
using MediatorForge.CQRS.Exceptions;
using MediatorForge.CQRS.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MediatorForge.CQRS.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Log the start of validation
        logger.LogInformation("Validating request={Request}", typeof(TRequest).Name);
        var validationResults = validators?.Any() == true ? await Task.WhenAll(
            validators
            .Select(validator =>
            validator.ValidateAsync(request)
            )) : null;

        var failures = validationResults?
               .Where(r => !r.IsValid)
               .SelectMany(r => r.Errors)
               .ToList();

        if (failures != null)
        {
            // Log the validation failure event
            logger.LogWarning("Validation failed for request {Request}. Errors: {Errors}", typeof(TRequest).Name, failures);
            throw new ValidationException(failures);
        }

        return await next();
    }
}
