namespace MediatorForge.Abstractions;

/// <summary>
/// Represents a handler that processes a request of type <typeparamref name="TRequest"/>
/// and produces a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <remarks>
/// <para>
/// This is the core abstraction for handling requests in the mediator pattern.
/// It serves as the base contract for all handlers, including command handlers
/// and query handlers.
/// </para>
/// <para>
/// Each request type must have exactly one corresponding handler implementation.
/// </para>
/// <para>
/// Implementations of this interface contain the business logic required to
/// process the request and return a response.
/// </para>
/// <para><b>Example:</b></para>
/// <code>
/// // Query
/// public sealed record GetUserByIdQuery(Guid UserId) : IQuery&lt;UserDto&gt;;
///
/// // Response DTO
/// public sealed record UserDto(Guid Id, string Name);
///
/// // Handler
/// public sealed class GetUserByIdHandler 
///     : IHandler&lt;GetUserByIdQuery, UserDto&gt;
/// {
///     public async Task&lt;UserDto&gt; HandleAsync(
///         GetUserByIdQuery request,
///         CancellationToken ct)
///     {
///         // Simulate fetching data (e.g., from database)
///         var user = new UserDto(request.UserId, "John Doe");
///
///         return await Task.FromResult(user);
///     }
/// }
///
/// // Usage
/// var user = await dispatcher.QueryAsync&lt;GetUserByIdQuery, UserDto&gt;(
///     new GetUserByIdQuery(userId));
/// </code>
/// </remarks>
public interface IHandler<in TRequest, TResponse>
     where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the incoming request and produces a response.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The response from the handler.</returns>
    Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);
}
