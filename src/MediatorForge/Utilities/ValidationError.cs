namespace MediatorForge.Utilities;

/// <summary>
/// Represents a validation error with a property name, error message, and attempted value.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Gets the name of the property that caused the validation error.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets the message that describes the validation error.
    /// </summary>
    public string ErrorMessage { get; }

    /// <summary>
    /// Gets the value that was attempted when the validation error occurred.
    /// </summary>
    public object AttemptedValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> class with the specified property name, error message, and attempted value.
    /// </summary>
    /// <param name="propertyName">The name of the property that caused the validation error.</param>
    /// <param name="errorMessage">The message that describes the validation error.</param>
    /// <param name="attemptedValue">The value that was attempted when the validation error occurred.</param>
    public ValidationError(string propertyName, string errorMessage, object attemptedValue)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
        AttemptedValue = attemptedValue;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"{PropertyName}: {ErrorMessage} (Attempted Value: {AttemptedValue})";
    }
}

