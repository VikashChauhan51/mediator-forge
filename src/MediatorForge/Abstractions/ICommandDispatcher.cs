namespace MediatorForge.Abstractions;

/// <summary>
/// Dispatcher for commands (write operations).
/// Responsible for sending commands to their respective handlers for processing.
/// </summary>
/// <remarks>
/// <para>
/// Commands represent operations that modify the state of the system (write operations).
/// Each command is handled by exactly one handler.
/// </para>
/// <para>
/// The dispatcher resolves the appropriate handler and executes any registered
/// pipeline behaviors before invoking the handler.
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
///         // Simulate creating an order
///         var orderId = Guid.NewGuid();
///
///         // Persist to database here
///
///         return await Task.FromResult(orderId);
///     }
/// }
///
/// // Usage
/// var orderId = await commandDispatcher.SendAsync&lt;CreateOrderCommand, Guid&gt;(
///     new CreateOrderCommand("John Doe"));
/// </code>
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
/// await commandDispatcher.SendAsync(
///     new DeleteOrderCommand(orderId));
/// </code>
/// </remarks>
public interface ICommandDispatcher
{
    /// <summary>
    /// Sends a command and returns a response. The command is processed by the appropriate command handler.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The response from the command handler.</returns>
    Task<TResponse> SendAsync<TCommand, TResponse>(
        TCommand command,
        CancellationToken ct = default)
        where TCommand : ICommand<TResponse>;

    /// <summary>
    /// Sends a command that does not expect a response (void).
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendAsync<TCommand>(
       TCommand command,
       CancellationToken ct = default)
       where TCommand : ICommand;
}
