using MediatR;

namespace MediatorForge.CQRS.Commands;

/// <summary>
/// Handle ICommand Request.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public interface ICommandHandler<in TCommand>
    : ICommandHandler<TCommand, Unit>
    where TCommand : ICommand<Unit>
{
}

/// <summary>
/// Handle ICommand Request.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface ICommandHandler<in TCommand, TResponse>
    : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{
}
