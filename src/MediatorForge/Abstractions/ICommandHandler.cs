namespace MediatorForge.Abstractions;

/// <summary>
/// Defines a handler for a command. A command represents a write operation that modifies system state
/// and expects a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <remarks>
/// <para>
/// Each command must have exactly one handler responsible for executing the business logic.
/// </para>
/// <para>
/// Command handlers are typically used for operations such as creating, updating, or deleting data.
/// </para>
/// <para><b>Example (Command with response):</b></para>
/// <code>
/// // Command
/// public sealed record CreateOrderCommand(string CustomerName) : ICommand&lt;Guid&gt;;
///
/// // Handler
/// public sealed class CreateOrderHandler 
///     : ICommandHandler&lt;CreateOrderCommand, Guid&gt;
/// {
///     public async Task&lt;Guid&gt; HandleAsync(
///         CreateOrderCommand command,
///         CancellationToken ct)
///     {
///         // Perform business logic
///         var orderId = Guid.NewGuid();
///
///         // Save to database
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
public interface ICommandHandler<in TCommand, TResponse> : IHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;


/// <summary>
/// Defines a handler for a command that does not expect a response (void).
/// This is a specialization of <see cref="ICommandHandler{TCommand, TResponse}"/> 
/// where the response type is <see cref="Unit"/>.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <remarks>
/// <para>
/// This handler is used for commands that perform an action without returning a result,
/// such as delete or update operations.
/// </para>
/// <para><b>Example (Command without response):</b></para>
/// <code>
/// // Command
/// public sealed record DeleteOrderCommand(Guid OrderId) : ICommand;
///
/// // Handler
/// public sealed class DeleteOrderHandler 
///     : ICommandHandler&lt;DeleteOrderCommand&gt;
/// {
///     public async Task&lt;Unit&gt; HandleAsync(
///         DeleteOrderCommand command,
///         CancellationToken ct)
///     {
///         // Perform delete operation
///
///         return Unit.Value;
///     }
/// }
///
/// // Usage
/// await dispatcher.SendAsync(
///     new DeleteOrderCommand(orderId));
/// </code>
/// </remarks>
public interface ICommandHandler<in TCommand> : IHandler<TCommand, Unit>
    where TCommand : ICommand<Unit>;
