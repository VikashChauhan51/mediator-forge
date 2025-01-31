using FluentValidation;
using FluentValidation.Results;
using MediatR;
using ResultifyCore;
using System.Reflection;

namespace MediatorForge.Behaviors;

/// <summary>
/// Behavior for validating requests using FluentValidation before handling them.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> :
  IBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse> where TResponse : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="validators">The validators to use for validating the request.</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Handles the request by validating it before passing it to the next handler.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="next">The next handler to call if validation succeeds.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The response from the next handler or a validation error response.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count > 0)
            {
                return typeof(TResponse).IsGenericType switch
                {
                    true when typeof(TResponse).GetGenericTypeDefinition() == typeof(Outcome<>) =>
                        CreateOutcomeResponse(failures),
                    true when typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>) =>
                        CreateResultResponse(failures),
                    true when typeof(TResponse).GetGenericTypeDefinition() == typeof(Option<>) =>
                        CreateOptionResponse(),
                    _ when typeof(TResponse) == typeof(Outcome) =>
                        (TResponse)(object)Outcome.Validation(failures.Select(x => OutcomeError(x))),
                    _ when typeof(TResponse) == typeof(Result) =>
                        (TResponse)(object)new ValidationException(failures).Validation(),
                    _ => throw new ValidationException(failures)
                };
            }
        }

        return await next();
    }

    /// <summary>
    /// Creates an outcome response for validation failures.
    /// </summary>
    /// <param name="failures">The validation failures.</param>
    /// <returns>An outcome response indicating validation errors.</returns>
    private TResponse CreateOutcomeResponse(List<ValidationFailure> failures)
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

        return (TResponse)constructor.Invoke
        (
            new object[]
            { ResultState.Validation,
                defaultValue,
                failures.Select(x => OutcomeError(x))
            }
        );
    }

    /// <summary>
    /// Creates a result response for validation failures.
    /// </summary>
    /// <param name="failures">The validation failures.</param>
    /// <returns>A result response indicating validation errors.</returns>
    private TResponse CreateResultResponse(List<ValidationFailure> failures)
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

        return (TResponse) constructor.Invoke
        (
            new object[]
            { ResultState.Validation,
                defaultValue!,
                new ValidationException(failures)
            }
        );
    }

    /// <summary>
    /// Creates an option response for validation failures.
    /// </summary>
    /// <returns>An option response indicating validation errors.</returns>
    private TResponse CreateOptionResponse()
    {
        var tOption = typeof(TRequest).GetGenericArguments()[0];
        var typeOfResult = tOption.GetGenericArguments()[0];
        var optionType = typeof(Option<>).MakeGenericType(typeOfResult);
        var noneProperty = optionType.GetProperty("None", BindingFlags.Static | BindingFlags.Public);
        return (TResponse)noneProperty.GetValue(null);
    }

    /// <summary>
    /// Converts a validation failure to an outcome error.
    /// </summary>
    /// <param name="validationFailure">The validation failure.</param>
    /// <returns>An outcome error representing the validation failure.</returns>
    private static OutcomeError OutcomeError(ValidationFailure validationFailure)
    {
        return new OutcomeError
        (
            validationFailure.PropertyName,
            validationFailure.ErrorCode,
            validationFailure.ErrorMessage,
            OutcomeErrorSeverity(validationFailure.Severity)
        );
    }

    /// <summary>
    /// Converts a FluentValidation severity to a validation severity.
    /// </summary>
    /// <param name="severity">The FluentValidation severity.</param>
    /// <returns>The corresponding validation severity.</returns>
    private static ValidationSeverity OutcomeErrorSeverity(Severity severity)
    {
        return severity switch
        {
            Severity.Error => ValidationSeverity.Error,
            Severity.Warning => ValidationSeverity.Warning,
            Severity.Info => ValidationSeverity.Info,
            _ => ValidationSeverity.Error
        };
    }
}
