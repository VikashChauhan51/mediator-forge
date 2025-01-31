using MediatorForge.Abstraction;
using MediatR;
using ResultifyCore;
using System.Reflection;


namespace MediatorForge.Behaviors;

/// <summary>
/// Behavior to handle authorization for requests.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public sealed class AuthorizationBehavior<TRequest, TResponse> : IBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> where TResponse : notnull
{
    private readonly IAuthorization _authorization;

    private const string UnauthorizedMessage = "Unauthorized";

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="authorization">The authorization service.</param>
    public AuthorizationBehavior(IAuthorization authorization)
    {
        _authorization = authorization;
    }

    /// <summary>
    /// Handles the authorization for the request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the authorization fails.</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizationResult = await _authorization.AuthorizeAsync();

        if (!authorizationResult.IsAuthorized)
        {
            return typeof(TResponse).IsGenericType switch
            {
                true when typeof(TResponse).GetGenericTypeDefinition() == typeof(Outcome<>) =>
                    CreateOutcomeResponse(authorizationResult.Reason ?? UnauthorizedMessage),
                true when typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>) =>
                    CreateResultResponse(authorizationResult.Reason ?? UnauthorizedMessage),
                true when typeof(TResponse).GetGenericTypeDefinition() == typeof(Option<>) =>
                     throw new UnauthorizedAccessException(authorizationResult.Reason),
                _ when typeof(TResponse) == typeof(Outcome) =>
                    (TResponse)(object)Outcome.Unauthorized(new OutcomeError(authorizationResult.Reason ?? UnauthorizedMessage)),
                _ when typeof(TResponse) == typeof(Result) =>
                    (TResponse)(object)new UnauthorizedAccessException(authorizationResult.Reason ?? UnauthorizedMessage).Unauthorized(),
                _ => throw new UnauthorizedAccessException(authorizationResult.Reason ?? UnauthorizedMessage)
            };
        }

        return await next();
    }

    /// <summary>
    /// Creates an unauthorized outcome response.
    /// </summary>
    /// <param name="reason">The reason for the unauthorized response.</param>
    /// <returns>The unauthorized outcome response.</returns>
    private TResponse CreateOutcomeResponse(string reason)
    {
        var tResult = typeof(TRequest).GetGenericArguments()[0];
        var typeOfResult = tResult.GetGenericArguments()[0];
        var outcomeType = typeof(Outcome<>).MakeGenericType(typeOfResult);
        var constructor = outcomeType.GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new[] { typeof(ResultState), typeOfResult, typeof(IEnumerable<OutcomeError>) },
            null);

        var defaultValue = typeOfResult.IsValueType ? Activator.CreateInstance(typeOfResult) : null;
        return (TResponse)constructor!.Invoke(new object[]
        {
            ResultState.Unauthorized,
            defaultValue,
            new[] { new OutcomeError(reason) }
        });
    }

    /// <summary>
    /// Creates an unauthorized result response.
    /// </summary>
    /// <param name="reason">The reason for the unauthorized response.</param>
    /// <returns>The unauthorized result response.</returns>
    private TResponse CreateResultResponse(string reason)
    {
        var tResult = typeof(TRequest).GetGenericArguments()[0];
        var typeOfResult = tResult.GetGenericArguments()[0];
        var resultType = typeof(Result<>).MakeGenericType(typeOfResult);
        var constructor = resultType.GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new[] { typeof(ResultState), typeOfResult, typeof(Exception) },
            null);

        var defaultValue = typeOfResult.IsValueType ? Activator.CreateInstance(typeOfResult) : null;
        return (TResponse)constructor!.Invoke(new object[]
        {
            ResultState.Unauthorized,
            defaultValue,
            new UnauthorizedAccessException(reason)
        })!;
    }
}
