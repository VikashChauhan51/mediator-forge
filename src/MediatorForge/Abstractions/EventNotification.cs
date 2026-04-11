namespace MediatorForge.Abstractions;

/// <summary>
/// Provides a concrete implementation of <see cref="IEventNotification{TEvent}"/>.
/// This class wraps an event instance and exposes it to event notification handlers.
/// </summary>
/// <typeparam name="TEvent">The type of the event.</typeparam>
/// <remarks>
/// <para>
/// This class is typically used internally by the dispatcher to wrap event data
/// before publishing it to all registered <see cref="IEventNotificationHandler{TEvent}"/> implementations.
/// </para>
/// <para>
/// It ensures a consistent abstraction for passing event data through the mediator pipeline,
/// allowing handlers to access the event via the <see cref="Event"/> property.
/// </para>
/// <para><b>Example:</b></para>
/// <code>
/// // Event
/// public sealed record OrderCreatedEvent(Guid OrderId, string CustomerName);
///
/// // Dispatcher internally wraps the event
/// var notification = new EventNotification&lt;OrderCreatedEvent&gt;(
///     new OrderCreatedEvent(orderId, "John Doe"));
///
/// // Event Handler 1 - Send Email
/// public sealed class SendEmailHandler 
///     : IEventNotificationHandler&lt;OrderCreatedEvent&gt;
/// {
///     public async Task HandleAsync(
///         IEventNotification&lt;OrderCreatedEvent&gt; notification,
///         CancellationToken ct)
///     {
///         var evt = notification.Event;
///
///         // Send email logic
///         await Task.CompletedTask;
///     }
/// }
///
/// // Event Handler 2 - Analytics
/// public sealed class AnalyticsHandler 
///     : IEventNotificationHandler&lt;OrderCreatedEvent&gt;
/// {
///     public async Task HandleAsync(
///         IEventNotification&lt;OrderCreatedEvent&gt; notification,
///         CancellationToken ct)
///     {
///         var evt = notification.Event;
///
///         // Track analytics
///         await Task.CompletedTask;
///     }
/// }
///
/// // Usage (preferred way)
/// await dispatcher.PublishAsync(
///     new OrderCreatedEvent(orderId, "John Doe"));
///
/// // Internally:
/// // Event → EventNotification&lt;TEvent&gt; → Handlers (fan-out)
/// </code>
/// </remarks>
public sealed class EventNotification<TEvent>(TEvent @event)
    : IEventNotification<TEvent> where TEvent : notnull
{
    /// <inheritdoc/>
    public TEvent Event { get; } = @event;
}
