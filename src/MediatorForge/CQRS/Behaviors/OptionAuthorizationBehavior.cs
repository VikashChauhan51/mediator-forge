using MediatorForge.CQRS.Interfaces;
using MediatorForge.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MediatorForge.CQRS.Behaviors;


/// <summary>
/// Represents a pipeline behavior that handles authorization for a request, returning <see cref="Option{T}"/> if authorization fails.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class OptionAuthorizationBehavior<TRequest, TResponse>
    (IEnumerable<IAuthorizer<TRequest>> authorizers,
    ILogger<OptionAuthorizationBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, Option<TResponse>>
    where TRequest : IRequest<Option<TResponse>>, IRequest
{

    /// <summary>
    /// Handles the authorization of the request and proceeds to the next delegate if authorization succeeds.
    /// </summary>
    /// <param name="request">The request to be authorized.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response from the next delegate in the pipeline or <see cref="Option{T}.None"/> if authorization fails.</returns>
    public async Task<Option<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Option<TResponse>> next, CancellationToken cancellationToken)
    {
        if (authorizers != null)
        {
            // Log the start of authorization
            logger.LogInformation("Authorizing request={Request}", typeof(TRequest).Name);

            foreach (var authorizer in authorizers)
            {
                var authorizationResult = await authorizer.AuthorizeAsync(request);
                if (!authorizationResult.IsAuthorized)
                {
                    // Log the authorization failure event
                    logger.LogWarning("Authorization failed for request {Request}. Errors: {Errors}", typeof(TRequest).Name, authorizationResult.Errors);
                    return Option<TResponse>.None;
                }
            }
        }

        return await next();
    }
}
