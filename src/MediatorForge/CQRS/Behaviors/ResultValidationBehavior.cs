﻿using MediatorForge.CQRS.Exceptions;
using MediatorForge.CQRS.Interfaces;
using MediatorForge.Results;
using MediatR;

namespace MediatorForge.CQRS.Behaviors;

public class ResultValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, Result<TResponse>>
    where TRequest : IRequest<Result<TResponse>>, IRequest
{

    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken cancellationToken)
    {
        foreach (var validator in validators)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var validationException = new ValidationException(validationResult.Errors);
                return Result<TResponse>.Fail(validationException);
            }
        }

        return await next();
    }
}

