using MediatorForge.Abstraction;
using MediatR;

namespace MediatorForge.Behaviors;

/// <summary>
/// Behavior to handle authorization for MediatR requests.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public sealed class AuthorizationExceptionBehavior<TRequest, TResponse> : IBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> where TResponse : notnull
{
    private readonly IAuthorization _authorization;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationExceptionBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="authorization">The authorization service.</param>
    public AuthorizationExceptionBehavior(IAuthorization authorization)
    {
        _authorization = authorization;
    }

    /// <summary>
    /// Handles the authorization check before proceeding to the next behavior or handler.
    /// </summary>
    /// <param name="request">The request instance.</param>
    /// <param name="next">The delegate to the next behavior or handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the next behavior or handler.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the authorization fails.</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizationResult = await _authorization.AuthorizeAsync();

        if (!authorizationResult.IsAuthorized)
        {
            throw new UnauthorizedAccessException(authorizationResult.Reason);
        }

        return await next();
    }
}
