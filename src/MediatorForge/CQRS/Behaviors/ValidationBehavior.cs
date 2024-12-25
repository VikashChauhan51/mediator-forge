﻿using MediatorForge.CQRS.Exceptions;
using MediatorForge.CQRS.Validators;
using ResultifyCore;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MediatorForge.CQRS.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
   where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Log the start of validation
        logger.LogInformation("Validating request={Request}", typeof(TRequest).Name);
        var validationResults = validators?.Any() == true
            ? await Task.WhenAll(
                validators.Select(validator => validator.ValidateAsync(request))
            )
            : null;

        var failures = validationResults?
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures?.Count > 0)
        {
            // Log the validation failure event
            logger.LogWarning("Validation failed for request {Request}. Errors: {Errors}", typeof(TRequest).Name, failures);
            var validationException = new ValidationException(failures);
            var responseType = typeof(TResponse);
            if (responseType.IsGenericType)
            {
                var genericTypeDefinition = responseType.GetGenericTypeDefinition();
                var genericTypeArguments = responseType.GenericTypeArguments;

                if (genericTypeDefinition == typeof(Result<>))
                {
                    return (TResponse)Activator.CreateInstance(typeof(Result<>).MakeGenericType(genericTypeArguments), validationException)!;
                }

                if (genericTypeDefinition == typeof(Option<>))
                {
                    return (TResponse)Activator.CreateInstance(typeof(Option<>).MakeGenericType(genericTypeArguments))!;
                }

                if (genericTypeDefinition == typeof(OneOf<,>) && genericTypeArguments.Length == 2 && typeof(Exception).IsAssignableFrom(genericTypeArguments[1]))
                {
                    var oneOfResponseType = typeof(OneOf<,>).MakeGenericType(genericTypeArguments);
                    var fromT2Method = oneOfResponseType.GetMethod("FromT2");
                    return (TResponse)fromT2Method!.Invoke(null, new object[] { validationException })!;
                }
            }
            throw validationException;
        }


        return await next();
    }

}
