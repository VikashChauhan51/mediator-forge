using MediatorForge.CQRS.Validators;
using MediatorForge.Utilities;

namespace MediatorForge.Adapters;
public class FluentValidatorBase<TRequest> : FluentValidation.AbstractValidator<TRequest>, IValidator<TRequest>
{
    new public async Task<ValidationResult> ValidateAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await base.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
        {
            return ValidationResult.Success;
        }

        var errors = validationResult
            .Errors
            .Select(e =>
            new ValidationError
            (
                e.PropertyName,
                e.ErrorMessage,
                e.AttemptedValue
            ));
        return ValidationResult.Failure(errors);
    }
}
