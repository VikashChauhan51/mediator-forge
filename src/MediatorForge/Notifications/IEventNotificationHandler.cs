using MediatR;

namespace MediatorForge.Notifications;

/// <summary>
/// Handle IEventNotification.
/// </summary>
/// <typeparam name="TEvent"></typeparam>
public interface IEventNotificationHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : INotification
{
}
