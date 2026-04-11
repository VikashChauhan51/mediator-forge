namespace MediatorForge.Abstractions;

/// <summary>
/// Defines a unified dispatcher for sending commands, queries, and publishing events.
/// The dispatcher is responsible for locating the appropriate handlers and invoking them,
/// as well as executing any registered pipeline behaviors.
/// </summary>
/// <remarks>
/// <para>
/// The dispatcher serves as the central entry point for interacting with the mediator.
/// It combines command execution (write operations), query execution (read operations),
/// and event publishing (notifications) into a single abstraction.
/// </para>
/// <para>
/// It enables decoupling between application layers by removing direct dependencies
/// between senders and handlers.
/// </para>
/// <para><b>Example:</b></para>
/// <code>
/// // Command
/// public sealed record CreateOrderCommand(string CustomerName) : ICommand&lt;Guid&gt;;
///
/// // Event
/// public sealed record OrderCreatedEvent(Guid OrderId);
///
/// // Query
/// public sealed record GetOrderByIdQuery(Guid OrderId) : IQuery&lt;OrderDto&gt;;
///
/// // Command Handler
/// public sealed class CreateOrderHandler 
///     : ICommandHandler&lt;CreateOrderCommand, Guid&gt;
/// {
///     private readonly IEventDispatcher _eventDispatcher;
///
///     public CreateOrderHandler(IEventDispatcher eventDispatcher)
///     {
///         _eventDispatcher = eventDispatcher;
///     }
///
///     public async Task&lt;Guid&gt; HandleAsync(
///         CreateOrderCommand command,
///         CancellationToken ct)
///     {
///         var orderId = Guid.NewGuid();
///
///         // Persist order here
///
///         // Publish event
///         await _eventDispatcher.PublishAsync(
///             new OrderCreatedEvent(orderId), ct);
///
///         return orderId;
///     }
/// }
///
/// // Query Handler
/// public sealed class GetOrderByIdHandler 
///     : IHandler&lt;GetOrderByIdQuery, OrderDto&gt;
/// {
///     public async Task&lt;OrderDto&gt; HandleAsync(
///         GetOrderByIdQuery query,
///         CancellationToken ct)
///     {
///         // Fetch data from database
///         return await Task.FromResult(
///             new OrderDto(query.OrderId, "John Doe", 1500m));
///     }
/// }
///
/// // Usage
/// // 1. Send command
/// var orderId = await dispatcher.SendAsync&lt;CreateOrderCommand, Guid&gt;(
///     new CreateOrderCommand("John Doe"));
///
/// // 2. Query data
/// var order = await dispatcher.QueryAsync&lt;GetOrderByIdQuery, OrderDto&gt;(
///     new GetOrderByIdQuery(orderId));
///
/// // 3. Publish event manually (optional)
/// await dispatcher.PublishAsync(
///     new OrderCreatedEvent(orderId));
/// </code>
/// </remarks>
public interface IDispatcher : ICommandDispatcher, IQueryDispatcher, IEventDispatcher
{
}
