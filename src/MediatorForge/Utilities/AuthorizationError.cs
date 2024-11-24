
namespace MediatorForge.Utilities;

/// <summary>
/// Represents an authorization error with a code, message, and additional data.
/// </summary>
public record AuthorizationError(string Code, string Message, object? AdditionalData = null)
{
    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"{Code}: {Message} (Additional Data: {AdditionalData})";
    }
}

