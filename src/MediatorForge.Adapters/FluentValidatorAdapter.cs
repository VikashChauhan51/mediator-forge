using MediatR;

namespace MediatorForge.Adapters;

public class FluentValidatorAdapter<TRequest> : CQRS.IValidator<TRequest>
    where TRequest : IRequest
{
    private readonly FluentValidation.IValidator<TRequest> _fluentValidator;

    public FluentValidatorAdapter(FluentValidation.IValidator<TRequest> fluentValidator)
    {
        _fluentValidator = fluentValidator;
    }

    public async Task<Results.ValidationResult> ValidateAsync(TRequest request)
    {
        var validationResult = await _fluentValidator.ValidateAsync(request);

        if (validationResult.IsValid)
        {
            return Results.ValidationResult.Success;
        }

        var errors = validationResult.Errors.Select(e => e.ErrorMessage);
        return Results.ValidationResult.Failure(errors);
    }
}
