namespace MediatorForge.Abstractions;

/// <summary>
/// Defines a handler for a command. A command is a request that expects a response. The response can be of any type, including void (Unit).
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;

/// <summary>
/// Defines a handler for a command that does not expect a response (void). This is a specialization of ICommandHandler where the response type is Unit.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
public interface ICommandHandler<in TCommand> : IHandler<TCommand, Unit>
    where TCommand : ICommand<Unit>;
