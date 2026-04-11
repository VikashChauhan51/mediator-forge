namespace MediatorForge.Abstractions;

/// <summary>
/// Defines a command that can be sent to a mediator for processing. 
/// A command represents an action or operation that should be performed, 
/// and it may optionally return a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <remarks>
/// <para>
/// Commands represent intent to change the state of the system (write operations).
/// Each command must have exactly one handler.
/// </para>
/// <para><b>Example (Command with response):</b></para>
/// <code>
/// // Command
/// public sealed record CreateOrderCommand(string CustomerName) : ICommand&lt;Guid&gt;;
///
/// // Handler
/// public sealed class CreateOrderHandler 
///     : IHandler&lt;CreateOrderCommand, Guid&gt;
/// {
///     public async Task&lt;Guid&gt; HandleAsync(
///         CreateOrderCommand command,
///         CancellationToken ct)
///     {
///         // Simulate persistence
///         var orderId = Guid.NewGuid();
///
///         // Save order to database here
///
///         return await Task.FromResult(orderId);
///     }
/// }
///
/// // Usage
/// var orderId = await dispatcher.SendAsync&lt;CreateOrderCommand, Guid&gt;(
///     new CreateOrderCommand("John Doe"));
/// </code>
/// </remarks>
public interface ICommand<TResponse> : IRequest<TResponse>;

/// <summary>
/// Defines a command that can be sent to a mediator for processing.
/// A command represents an action or operation that should be performed,
/// and it does not return a response.
/// </summary>
/// <remarks>
/// <para>
/// This is a convenience abstraction for commands that do not return a value.
/// Internally, it uses <see cref="Unit"/> as the response type.
/// </para>
/// <para><b>Example (Command without response):</b></para>
/// <code>
/// // Command
/// public sealed record DeleteOrderCommand(Guid OrderId) : ICommand;
///
/// // Handler
/// public sealed class DeleteOrderHandler 
///     : IHandler&lt;DeleteOrderCommand, Unit&gt;
/// {
///     public async Task&lt;Unit&gt; HandleAsync(
///         DeleteOrderCommand command,
///         CancellationToken ct)
///     {
///         // Delete order from database
///
///         return Unit.Value;
///     }
/// }
///
/// // Usage
/// await dispatcher.SendAsync(new DeleteOrderCommand(orderId));
/// </code>
/// </remarks>
public interface ICommand : ICommand<Unit>;
