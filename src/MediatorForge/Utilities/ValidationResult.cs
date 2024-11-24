namespace MediatorForge.Utilities;

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the validation was successful.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Gets the collection of validation errors.
    /// </summary>
    public IEnumerable<ValidationError> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResult"/> class with a collection of validation errors.
    /// </summary>
    /// <param name="errors">The collection of validation errors.</param>
    public ValidationResult(IEnumerable<ValidationError> errors)
    {
        Errors = errors;
        IsValid = !(errors?.Any() ?? false);
    }

    /// <summary>
    /// Gets a <see cref="ValidationResult"/> that represents a successful validation.
    /// </summary>
    public static ValidationResult Success => new ValidationResult(Enumerable.Empty<ValidationError>());

    /// <summary>
    /// Creates a <see cref="ValidationResult"/> that represents a failed validation with the specified errors.
    /// </summary>
    /// <param name="errors">The collection of validation errors.</param>
    /// <returns>A <see cref="ValidationResult"/> that represents a failed validation.</returns>
    public static ValidationResult Failure(IEnumerable<ValidationError> errors) => new ValidationResult(errors);
}

