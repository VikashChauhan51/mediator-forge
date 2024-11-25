using MediatorForge.CQRS.Commands;
using MediatorForge.CQRS.Exceptions;
using MediatorForge.CQRS.Interfaces;
using MediatorForge.CQRS.Queries;
using MediatorForge.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MediatorForge.CQRS.Behaviors;

public class ResultValidationBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators,
    ILogger<ResultValidationBehavior<TRequest, TResponse>> logger) : IResultPipelineBehavior<TRequest, TResponse>
    where TRequest : IResultCommand<TResponse>
{
    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken cancellationToken)
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
                    var validationException = new ValidationException(validationResult.Errors);
                    return Result<TResponse>.Fail(validationException);
                }
            }
        }
        return await next();
    }
}
