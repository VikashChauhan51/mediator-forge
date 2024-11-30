using MediatorForge.CQRS.Exceptions;
using MediatorForge.CQRS.Validators;
using MediatorForge.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MediatorForge.CQRS.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
   where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Log the start of validation
        logger.LogInformation("Validating request={Request}", typeof(TRequest).Name);
        var validationResults = validators?.Any() == true
            ? await Task.WhenAll(
                validators.Select(validator => validator.ValidateAsync(request))
            )
            : null;

        var failures = validationResults?
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures?.Count > 0)
        {
            // Log the validation failure event
            logger.LogWarning("Validation failed for request {Request}. Errors: {Errors}", typeof(TRequest).Name, failures);
            var validationException = new ValidationException(failures);
            var responseType = typeof(TResponse);
            return responseType.IsGenericType
               ? responseType.GetGenericTypeDefinition() switch
               {
                   var genericType when genericType == typeof(Result<>) =>
                       (TResponse)Activator.CreateInstance(typeof(Result<>).MakeGenericType(responseType.GenericTypeArguments), validationException)!,

                   var genericType when genericType == typeof(Option<>) =>
                       (TResponse)Activator.CreateInstance(typeof(Option<>).MakeGenericType(responseType.GenericTypeArguments))!,

                   _ => throw validationException
               }
               : throw validationException;
        }


        return await next();
    }

}
