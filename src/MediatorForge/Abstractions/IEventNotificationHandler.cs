
namespace MediatorForge.Abstractions;

/// <summary>
/// Handle IEventNotification.
/// </summary>
/// <typeparam name="TEvent"></typeparam>
public interface IEventNotificationHandler<in TEvent>
    where TEvent : notnull
{
    /// <summary>
    /// Handles the event notification asynchronously. The method takes an instance of <see cref="IEventNotification{TEvent}"/> containing the event data and a cancellation token. Implementations of this method should contain the logic to process the event notification, such as updating state, triggering side effects, or sending messages to other systems.
    /// </summary>
    /// <param name="notification">The event notification to handle.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(IEventNotification<TEvent> notification, CancellationToken ct = default);
}
