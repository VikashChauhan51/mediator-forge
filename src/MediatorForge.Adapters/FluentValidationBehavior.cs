using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using ResultifyCore;

namespace MediatorForge.Adapters;
public class FluentValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<FluentValidationBehavior<TRequest, TResponse>> _logger;

    public FluentValidationBehavior(IEnumerable<IValidator<TRequest>> validators,
        ILogger<FluentValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Log the start of validation
        _logger.LogInformation("Validating request={Request}", typeof(TRequest).Name);

        var validationResults = _validators?.Any() == true
            ? await Task.WhenAll(
                _validators.Select(validator => validator.ValidateAsync(request, cancellationToken))
            )
            : null;

        var failures = validationResults?
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures?.Count > 0)
        {
            // Log the validation failure event
            _logger.LogWarning("Validation failed for request {Request}. Errors: {Errors}", typeof(TRequest).Name, failures);

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

