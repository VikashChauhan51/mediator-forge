
namespace MediatorForge.Results;

public class ValidationResult
{
    public bool IsValid { get; }
    public IEnumerable<string> Errors { get; }

    public ValidationResult(IEnumerable<string> errors)
    {
        Errors = errors;
        IsValid = !errors.Any();
    }

    public static ValidationResult Success => new ValidationResult(Enumerable.Empty<string>());

    public static ValidationResult Failure(IEnumerable<string> errors) => new ValidationResult(errors);
}

