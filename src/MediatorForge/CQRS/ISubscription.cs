using MediatR;

namespace MediatorForge.CQRS;

/// <summary>
/// Execute ISubscription.
/// </summary>
public interface ISubscription : ISubscription<Unit>
{
}

/// <summary>
/// Execute ISubscription.
/// </summary>
public interface ISubscription<out TResponse> : IRequest<TResponse>
{
}