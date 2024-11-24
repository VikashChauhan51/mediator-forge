using MediatR;

namespace MediatorForge.CQRS.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse>
    (IEnumerable<IAuthorizer<TRequest>> authorizers) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IRequest
{

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        foreach (var authorizer in authorizers)
        {
            await authorizer.AuthorizeAsync(request);
        }

        return await next();
    }
}

