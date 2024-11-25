namespace MediatorForge.Utilities;

/// <summary>
/// Represents a validation error with a property name, error message, and attempted value.
/// </summary>
public record ValidationError(string PropertyName, string ErrorMessage, object? AttemptedValue = null)
{
    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"{PropertyName}: {ErrorMessage} (Attempted Value: {AttemptedValue})";
    }
}

