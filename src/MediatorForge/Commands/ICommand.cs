using MediatR;

namespace MediatorForge.Commands;

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
