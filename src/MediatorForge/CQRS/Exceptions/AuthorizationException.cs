using MediatorForge.Utilities;

namespace MediatorForge.CQRS.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an authorization check fails.
/// </summary>
public class AuthorizationException : Exception
{
    /// <summary>
    /// Gets the collection of authorization errors.
    /// </summary>
    public IEnumerable<AuthorizationError> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationException"/> class with a specified error message
    /// and a collection of authorization errors.
    /// </summary>
    /// <param name="errors">The collection of authorization errors.</param>
    public AuthorizationException(IEnumerable<AuthorizationError> errors)
        : base("Authorization failed.")
    {
        Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationException"/> class with a specified error message
    /// and a collection of authorization errors.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errors">The collection of authorization errors.</param>
    public AuthorizationException(string message, IEnumerable<AuthorizationError> errors)
        : base(message)
    {
        Errors = errors;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"{Message}: {string.Join(", ", Errors)}";
    }
}

