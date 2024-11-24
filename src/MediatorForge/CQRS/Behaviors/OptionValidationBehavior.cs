using MediatorForge.CQRS.Interfaces;
using MediatorForge.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MediatorForge.CQRS.Behaviors;


/// <summary>
/// Represents a pipeline behavior that handles validation for a request, returning <see cref="Option{T}"/> if validation fails.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class OptionValidationBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators,
    ILogger<OptionValidationBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, Option<TResponse>>
    where TRequest : IRequest<Option<TResponse>>, IRequest
{

    /// <summary>
    /// Handles the validation of the request and proceeds to the next delegate if validation succeeds.
    /// </summary>
    /// <param name="request">The request to be validated.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response from the next delegate in the pipeline or <see cref="Option{T}.None"/> if validation fails.</returns>
    public async Task<Option<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Option<TResponse>> next, CancellationToken cancellationToken)
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
                    return Option<TResponse>.None;
                }
            }
        }

        return await next();
    }
}
