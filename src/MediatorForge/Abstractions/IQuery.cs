namespace MediatorForge.Abstractions;

/// <summary>
/// Represents a query that can be sent through a mediator to receive a response of type <typeparamref name="TResponse"/>.
/// This interface is used to define the structure of queries in the mediator pattern, allowing for decoupling between the sender and receiver of the query.
/// Implementing this interface indicates that a class is a query that can be processed by a query handler to produce a response.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <remarks>
/// <para>
/// Queries represent read-only operations and should not modify application state.
/// Each query must have exactly one handler that returns the requested data.
/// </para>
/// <para><b>Example:</b></para>
/// <code>
/// // Query
/// public sealed record GetOrderByIdQuery(Guid OrderId) : IQuery&lt;OrderDto&gt;;
///
/// // Response DTO
/// public sealed record OrderDto(Guid Id, string CustomerName, decimal TotalAmount);
///
/// // Handler
/// public sealed class GetOrderByIdHandler 
///     : IHandler&lt;GetOrderByIdQuery, OrderDto&gt;
/// {
///     public async Task&lt;OrderDto&gt; HandleAsync(
///         GetOrderByIdQuery query,
///         CancellationToken ct)
///     {
///         // Simulate data retrieval (e.g., from database)
///         var order = new OrderDto(
///             query.OrderId,
///             "John Doe",
///             1500m);
///
///         return await Task.FromResult(order);
///     }
/// }
///
/// // Usage
/// var order = await dispatcher.QueryAsync&lt;GetOrderByIdQuery, OrderDto&gt;(
///     new GetOrderByIdQuery(orderId));
/// </code>
/// </remarks>
public interface IQuery<TResponse> : IRequest<TResponse>;
