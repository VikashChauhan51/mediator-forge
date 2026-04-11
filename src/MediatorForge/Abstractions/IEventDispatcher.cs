namespace MediatorForge.Abstractions;

/// <summary>
/// Dispatcher for events (notifications).
/// </summary>
public interface IEventDispatcher
{
    /// <summary>
    /// Publishes an event to all registered handlers.
    ///  The event is processed by all handlers that are subscribed to the event type.
    /// This method is used for events, which represent something that has happened in the system and may be of interest to multiple handlers.
    ///  The PublishAsync method allows for a one-to-many relationship between events and handlers, while the SendAsync and QueryAsync methods are used for one-to-one relationships between commands/queries and their respective handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="eventData">The event data.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync<TEvent>(TEvent eventData, CancellationToken ct = default) where TEvent : notnull;
}
