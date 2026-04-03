namespace MediatorForge.Abstractions;

/// <summary>
/// Defines a command that can be sent to a mediator for processing. A command represents an action or operation that should be performed, and it may optionally return a response of type TResponse.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface ICommand<TResponse> : IRequest<TResponse>;

/// <summary>
/// Defines a command that can be sent to a mediator for processing. A command represents an action or operation that should be performed, and it does not return a response.
/// </summary>
public interface ICommand : ICommand<Unit>;
