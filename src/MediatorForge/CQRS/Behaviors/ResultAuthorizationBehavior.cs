using MediatorForge.CQRS.Interfaces;
using MediatorForge.Results;
using MediatorForge.CQRS.Exceptions;
using MediatR;

namespace MediatorForge.CQRS.Behaviors;
public class ResultAuthorizationBehavior<TRequest, TResponse>(IEnumerable<IAuthorizer<TRequest>> validators) : IPipelineBehavior<TRequest, Result<TResponse>>
    where TRequest : IRequest<Result<TResponse>>, IRequest
{

    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken cancellationToken)
    {
        foreach (var validator in validators)
        {
            var validationResult = await validator.AuthorizeAsync(request);
            if (!validationResult.IsAuthorized)
            {
                var validationException = new AuthorizationException(validationResult.Errors);
                return Result<TResponse>.Fail(validationException);
            }
        }

        return await next();
    }
}
