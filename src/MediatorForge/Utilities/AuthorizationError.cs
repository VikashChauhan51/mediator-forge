
namespace MediatorForge.Utilities;

/// <summary>
/// Represents an authorization error with a code, message, and additional data.
/// </summary>
public class AuthorizationError
{
    /// <summary>
    /// Gets the code that identifies the authorization error.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the message that describes the authorization error.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets any additional data associated with the authorization error.
    /// </summary>
    public object? AdditionalData { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationError"/> class with the specified code, message, and additional data.
    /// </summary>
    /// <param name="code">The code that identifies the authorization error.</param>
    /// <param name="message">The message that describes the authorization error.</param>
    /// <param name="additionalData">Any additional data associated with the authorization error.</param>
    public AuthorizationError(string code, string message, object? additionalData = null)
    {
        Code = code;
        Message = message;
        AdditionalData = additionalData;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"{Code}: {Message} (Additional Data: {AdditionalData})";
    }
}

