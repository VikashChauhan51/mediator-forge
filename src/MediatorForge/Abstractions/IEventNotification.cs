namespace MediatorForge.Abstractions;

/// <summary>
/// Represents a notification wrapper for an event that can be published through a mediator.
/// This abstraction allows events to be handled by one or more notification handlers,
/// enabling a decoupled publish-subscribe model.
/// </summary>
/// <typeparam name="TEvent">The event type.</typeparam>
/// <remarks>
/// <para>
/// Events represent something that has already happened in the system.
/// Unlike commands or queries, events can have multiple handlers (fan-out).
/// </para>
/// <para>
/// The <see cref="IEventNotification{TEvent}"/> acts as a wrapper around the event data,
/// allowing it to be passed through the mediator and processed by all registered handlers.
/// </para>
/// <para><b>Example:</b></para>
/// <code>
/// // Event
/// public sealed record OrderCreatedEvent(Guid OrderId, string CustomerName);
///
/// // Event Notification (usually created internally by dispatcher)
/// var notification = new EventNotification&lt;OrderCreatedEvent&gt;(
///     new OrderCreatedEvent(orderId, "John Doe"));
///
/// // Event Handler 1 - Send Email
/// public sealed class SendEmailOnOrderCreatedHandler 
///     : IEventNotificationHandler&lt;OrderCreatedEvent&gt;
/// {
///     public async Task HandleAsync(
///         IEventNotification&lt;OrderCreatedEvent&gt; notification,
///         CancellationToken ct)
///     {
///         var evt = notification.Event;
///
///         // Send email logic here
///         await Task.CompletedTask;
///     }
/// }
///
/// // Event Handler 2 - Audit Logging
/// public sealed class AuditOrderCreatedHandler 
///     : IEventNotificationHandler&lt;OrderCreatedEvent&gt;
/// {
///     public async Task HandleAsync(
///         IEventNotification&lt;OrderCreatedEvent&gt; notification,
///         CancellationToken ct)
///     {
///         var evt = notification.Event;
///
///         // Log audit information here
///         await Task.CompletedTask;
///     }
/// }
///
/// // Usage (publishing event)
/// await dispatcher.PublishAsync(
///     new OrderCreatedEvent(orderId, "John Doe"));
///
/// // Both handlers will be executed
/// </code>
/// </remarks>
public interface IEventNotification<out TEvent> where TEvent : notnull
{
    /// <summary>
    /// Gets the event data.
    /// </summary>
    TEvent Event { get; }
}
