using MediatorForge.CQRS.Validators;
using MediatorForge.Utilities;
using MediatR;

namespace MediatorForge.Adapters;

/// <summary>
/// Adapter class to integrate FluentValidation validators with the custom validation system.
/// </summary>
/// <typeparam name="TRequest">The type of the request to be validated.</typeparam>
public class FluentValidatorAdapter<TRequest>(FluentValidation.IValidator<TRequest> fluentValidator) : IValidator<TRequest>
    where TRequest : IRequest
{

    /// <inheritdoc/>
    /// <returns>A task that represents the asynchronous validation operation. The task result contains the <see cref="ValidationResult"/>.</returns>
    public async Task<ValidationResult> ValidateAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await fluentValidator.ValidateAsync(request);

        if (validationResult.IsValid)
        {
            return ValidationResult.Success;
        }

        var errors = validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage, e.AttemptedValue));
        return ValidationResult.Failure(errors);
    }
}
