namespace MediatorForge.Abstractions;

/// <summary>
/// Defines a dispatcher for sending commands and queries. The dispatcher is responsible for finding the appropriate handler for a given command or query and invoking it. The dispatcher can also handle cross-cutting concerns such as logging, validation, and error handling.
/// </summary>
public interface IDispatcher : ICommandDispatcher, IQueryDispatcher, IEventDispatcher
{
}
