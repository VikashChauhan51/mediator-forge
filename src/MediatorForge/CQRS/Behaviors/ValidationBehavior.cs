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
        if (validators != null)
        {
            // Log the start of validation
            logger.LogInformation("Validating request={Request}", typeof(TRequest).Name);
            foreach (var validator in validators)
            {
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    // Log the validation failure event
                    logger.LogWarning("Validation failed for request {Request}. Errors: {Errors}", typeof(TRequest).Name, validationResult.Errors);
                    throw new ValidationException(validationResult.Errors);
                }
            }
        }
        return await next();
    }
}
