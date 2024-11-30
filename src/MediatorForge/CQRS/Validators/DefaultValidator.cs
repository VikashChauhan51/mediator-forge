using MediatorForge.Utilities;

namespace MediatorForge.CQRS.Validators;

/// <summary>
/// The default validation implementation.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public class DefaultValidator<TRequest> : IValidator<TRequest>
{
    public Task<ValidationResult> ValidateAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        // Always validate successfully
        return Task.FromResult(ValidationResult.Success);
    }
}

