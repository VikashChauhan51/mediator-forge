using MediatR;

namespace MediatorForge.CQRS;

/// <summary>
/// Handle INotification.
/// </summary>
/// <typeparam name="TNotification"></typeparam>
public interface INotificationHandler<in TNotification>
    : IRequestHandler<TNotification, Unit>
    where TNotification : INotification<Unit>
{
}

/// <summary>
/// Handle INotification.
/// </summary>
/// <typeparam name="TNotification"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface INotificationHandler<in TNotification, TResponse>
    : IRequestHandler<TNotification, TResponse>
    where TNotification : INotification<TResponse>
    where TResponse : notnull
{
}