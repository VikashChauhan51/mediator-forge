namespace MediatorForge.Abstractions;

/// <summary>
/// The <see cref="EventNotification{TEvent}"/> class is a concrete implementation of the <see cref="IEventNotification{TEvent}"/> interface.
///  It encapsulates an event of type TEvent and provides a property to access the event data.
///  This class can be used to create event notifications that can be handled by implementations of <see cref="IEventNotificationHandler{TEvent}"/>.
///  The constructor takes an instance of TEvent and assigns it to the Event property, ensuring that the event data is available for processing when the notification is handled.
/// </summary>
/// <typeparam name="TEvent"></typeparam>
public sealed class EventNotification<TEvent>(TEvent @event) : IEventNotification<TEvent> where TEvent : notnull
{
    /// <inheritdoc/>
    public TEvent Event { get; } = @event;
}
