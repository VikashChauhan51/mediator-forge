using MediatorForge.Commands;

namespace MediatorForge.Behaviors;

/// <summary>
/// Interface for Command Pipeline Behaviors.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface ICommandBehavior<in TCommand, TResponse> : IBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{
}

