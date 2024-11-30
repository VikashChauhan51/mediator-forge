using MediatorForge.CQRS.Authorizers;
using MediatorForge.CQRS.Exceptions;
using MediatorForge.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MediatorForge.CQRS.Behaviors;

/// <summary>
/// Represents a pipeline behavior that handles authorization for a request.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class AuthorizationBehavior<TRequest, TResponse>
    (IAuthorizer<TRequest> authorizer,
    ILogger<AuthorizationBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{

    /// <summary>
    /// Handles the authorization of the request and proceeds to the next delegate if authorization succeeds.
    /// </summary>
    /// <param name="request">The request to be authorized.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response from the next delegate in the pipeline.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        // Log the start of authorization
        logger.LogInformation("Authorizing request={Request}", typeof(TRequest).Name);
        var authorizationResult = await authorizer.AuthorizeAsync(request, cancellationToken);
        if (authorizationResult != null &&
            !authorizationResult.IsAuthorized)
        {
            // Log the authorization failure event
            logger.LogWarning("Authorization failed for request {Request}. Errors: {Errors}", typeof(TRequest).Name, authorizationResult.Errors);
            var validationException = new AuthorizationException(authorizationResult.Errors);
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

        // Proceed to the next delegate in the pipeline
        return await next();
    }
}
