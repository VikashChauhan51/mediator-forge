using MediatR;

namespace MediatorForge.CQRS.Notifications;

/// <summary>
/// The generic implementation of event notification.
/// </summary>
/// <typeparam name="TEvent">The event type.</typeparam>
public interface IEventNotification<out TEvent> : INotification where TEvent : notnull
{
    /// <summary>
    /// Gets the event data.
    /// </summary>
    TEvent Event { get; }
}

