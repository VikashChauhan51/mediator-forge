using MediatR;

namespace MediatorForge.CQRS;

/// <summary>
/// Execute ICommand.
/// </summary>
public interface ICommand : ICommand<Unit>
{
}

/// <summary>
/// Execute ICommand.
/// </summary>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}