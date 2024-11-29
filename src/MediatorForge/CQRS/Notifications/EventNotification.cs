using MediatR;

namespace MediatorForge.CQRS.Notifications;

/// <summary>
/// The generic implementation of event notification.
/// </summary>
/// <typeparam name="TEvent">The event type of <see cref="TEvent"/>.</typeparam>
/// <param name="Event">The event data <see cref="TEvent"/>.</param>
public record EventNotification<TEvent>
(
    TEvent Event
) : INotification;

