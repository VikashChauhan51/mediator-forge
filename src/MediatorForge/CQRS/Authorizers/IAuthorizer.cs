using MediatorForge.Utilities;
using MediatR;

namespace MediatorForge.CQRS.Authorizers;

/// <summary>
/// Defines a contract for authorizing a specific request.
/// </summary>
/// <typeparam name="TRequest">The type of the request to authorize.</typeparam>
public interface IAuthorizer<in TRequest>
{
    /// <summary>
    /// Asynchronously authorizes the specified request.
    /// </summary>
    /// <param name="request">The request to authorize.</param>
    /// <param name="cancellationToken">The request cancellation token.</param>
    /// <returns>A task that represents the asynchronous authorization operation. The task result contains the <see cref="AuthorizationResult"/>.</returns>
    Task<AuthorizationResult> AuthorizeAsync(TRequest request, CancellationToken cancellationToken = default);
}


