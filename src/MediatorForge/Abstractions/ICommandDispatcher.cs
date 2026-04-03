
namespace MediatorForge.Abstractions;

/// <summary>
/// Dispatcher for commands (write operations).
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Sends a command and returns a response. The command is processed by the appropriate command handler, which can return a response of any type, including void (Unit). If the command does not expect a response, the SendAsync method with a void response type can be used instead.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The response from the command handler.</returns>
    /// <returns></returns>
    Task<TResponse> SendAsync<TCommand, TResponse>(
        TCommand command,
        CancellationToken ct = default)
        where TCommand : ICommand<TResponse>;

    /// <summary>
    /// Sends a command that does not expect a response (void). This is a specialization of the SendAsync method where the response type is Unit. The command is processed by the appropriate command handler, which does not return a response.
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
