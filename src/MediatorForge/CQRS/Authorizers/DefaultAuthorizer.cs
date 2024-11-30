using MediatorForge.Utilities;

namespace MediatorForge.CQRS.Authorizers;

/// <summary>
/// The default authorize implementation.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public class DefaultAuthorizer<TRequest> : IAuthorizer<TRequest>
{
    public Task<AuthorizationResult> AuthorizeAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        // Always authorize successfully
        return Task.FromResult(AuthorizationResult.Success);
    }
}

