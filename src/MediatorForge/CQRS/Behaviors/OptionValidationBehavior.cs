using MediatorForge.Results;
using MediatR;

namespace MediatorForge.CQRS.Behaviors;

public class OptionValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, Option<TResponse>>
    where TRequest : IRequest<Option<TResponse>>, IRequest
{

    public async Task<Option<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Option<TResponse>> next, CancellationToken cancellationToken)
    {
        foreach (var validator in validators)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Option<TResponse>.None;
            }
        }

        return await next();
    }
}

