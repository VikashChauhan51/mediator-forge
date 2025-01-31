namespace MediatorForge.Abstraction;

/// <summary>
/// Defines a method to authorize an action asynchronously.
/// </summary>
public interface IAuthorization
{
    /// <summary>
    /// Asynchronously authorizes an action.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the authorization result.</returns>
    Task<AuthorizationResult> AuthorizeAsync();
}


/// <summary>  
/// Represents the result of an authorization check.  
/// </summary>  
public sealed record AuthorizationResult (bool IsAuthorized, string? Reason);

