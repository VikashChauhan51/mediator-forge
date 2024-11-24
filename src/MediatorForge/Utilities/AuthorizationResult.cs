
namespace MediatorForge.Utilities;

/// <summary>
/// Represents the result of an authorization operation.
/// </summary>
public class AuthorizationResult
{
    /// <summary>
    /// Gets a value indicating whether the authorization was successful.
    /// </summary>
    public bool IsAuthorized { get; }

    /// <summary>
    /// Gets the collection of authorization errors.
    /// </summary>
    public IEnumerable<AuthorizationError> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationResult"/> class with a collection of authorization errors.
    /// </summary>
    /// <param name="errors">The collection of authorization errors.</param>
    public AuthorizationResult(IEnumerable<AuthorizationError> errors)
    {
        Errors = errors;
        IsAuthorized = !(errors?.Any() ?? false);
    }

    /// <summary>
    /// Gets an <see cref="AuthorizationResult"/> that represents a successful authorization.
    /// </summary>
    public static AuthorizationResult Success => new AuthorizationResult(Enumerable.Empty<AuthorizationError>());

    /// <summary>
    /// Creates an <see cref="AuthorizationResult"/> that represents a failed authorization with the specified errors.
    /// </summary>
    /// <param name="errors">The collection of authorization errors.</param>
    /// <returns>An <see cref="AuthorizationResult"/> that represents a failed authorization.</returns>
    public static AuthorizationResult Failure(IEnumerable<AuthorizationError> errors) => new AuthorizationResult(errors);
}

