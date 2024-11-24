using MediatorForge.CQRS.Interfaces;
using MediatorForge.Results;
using MediatR;

namespace MediatorForge.CQRS.Behaviors;


public class OptionAuthorizationBehavior<TRequest, TResponse>(IEnumerable<IAuthorizer<TRequest>> authorizers) : IPipelineBehavior<TRequest, Option<TResponse>>
    where TRequest : IRequest<Option<TResponse>>, IRequest
{


    public async Task<Option<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Option<TResponse>> next, CancellationToken cancellationToken)
    {
        foreach (var authorizer in authorizers)
        {
            var authorizationResult = await authorizer.AuthorizeAsync(request);
            if (!authorizationResult.IsAuthorized)
            {
                return Option<TResponse>.None;
            }
        }

        return await next();
    }
}
