namespace MediatorForge.Abstractions;

/// <summary>
/// Represents a request that can be sent through a mediator to receive a response of type <typeparamref name="TResponse"/>.
/// This interface defines the base contract for all requests in the mediator pattern, enabling decoupling between the sender and the handler.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <remarks>
/// <para>
/// A request is a generic abstraction that can represent either a command (write operation)
/// or a query (read operation). It is processed by a corresponding handler that produces a response.
/// </para>
/// <para>
/// Typically, you should use more specific abstractions such as <c>ICommand&lt;TResponse&gt;</c>
/// or <c>IQuery&lt;TResponse&gt;</c> for better semantic clarity.
/// </para>
/// <para><b>Example:</b></para>
/// <code>
/// // Generic request (can be treated as command or query)
/// public sealed record PingRequest(string Message) : IRequest&lt;string&gt;;
///
/// // Handler
/// public sealed class PingHandler 
///     : IHandler&lt;PingRequest, string&gt;
/// {
///     public async Task&lt;string&gt; HandleAsync(
///         PingRequest request,
///         CancellationToken ct)
///     {
///         // Simple processing logic
///         return await Task.FromResult($"Pong: {request.Message}");
///     }
/// }
///
/// // Usage
/// var response = await dispatcher.SendAsync&lt;PingRequest, string&gt;(
///     new PingRequest("Hello"));
/// </code>
/// </remarks>
public interface IRequest<TResponse>;
