using MediatR;

namespace MediatorForge.CQRS;

/// <summary>
/// Execute INotification.
/// </summary>
public interface INotification : INotification<Unit>
{
}

/// <summary>
/// Execute INotification.
/// </summary>
public interface INotification<out TResponse> : IRequest<TResponse>
{
}
