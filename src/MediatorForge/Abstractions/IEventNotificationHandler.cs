namespace MediatorForge.Abstractions;

/// <summary>
/// Defines a handler for processing event notifications.
/// Event notification handlers are invoked when an event is published through the dispatcher.
/// </summary>
/// <typeparam name="TEvent">The type of the event.</typeparam>
/// <remarks>
/// <para>
/// Event handlers are used to react to events that have already occurred in the system.
/// Unlike commands and queries, multiple handlers can be registered for the same event (fan-out).
/// </para>
/// <para>
/// Event handlers should be side-effect oriented, such as sending notifications,
/// updating read models, logging, or integrating with external systems.
/// </para>
/// <para><b>Example:</b></para>
/// <code>
/// // Event
/// public sealed record OrderCreatedEvent(Guid OrderId, string CustomerName);
///
/// // Handler 1 - Send Email
/// public sealed class SendEmailHandler 
///     : IEventNotificationHandler&lt;OrderCreatedEvent&gt;
/// {
///     public async Task HandleAsync(
///         IEventNotification&lt;OrderCreatedEvent&gt; notification,
///         CancellationToken ct)
///     {
///         var evt = notification.Event;
///
///         // Send email to customer
///         Console.WriteLine($"Email sent for order {evt.OrderId}");
///
///         await Task.CompletedTask;
///     }
/// }
///
/// // Handler 2 - Audit Logging
/// public sealed class AuditHandler 
///     : IEventNotificationHandler&lt;OrderCreatedEvent&gt;
/// {
///     public async Task HandleAsync(
///         IEventNotification&lt;OrderCreatedEvent&gt; notification,
///         CancellationToken ct)
///     {
///         var evt = notification.Event;
///
///         // Log audit entry
///         Console.WriteLine($"Audit log created for order {evt.OrderId}");
///
///         await Task.CompletedTask;
///     }
/// }
///
/// // Handler 3 - Analytics
/// public sealed class AnalyticsHandler 
///     : IEventNotificationHandler&lt;OrderCreatedEvent&gt;
/// {
///     public async Task HandleAsync(
///         IEventNotification&lt;OrderCreatedEvent&gt; notification,
///         CancellationToken ct)
///     {
///         var evt = notification.Event;
///
///         // Send data to analytics system
///         Console.WriteLine($"Analytics tracked for order {evt.OrderId}");
///
///         await Task.CompletedTask;
///     }
/// }
///
/// // Usage
/// await dispatcher.PublishAsync(
///     new OrderCreatedEvent(orderId, "John Doe"));
///
/// // All handlers will be executed (fan-out)
/// </code>
/// </remarks>
public interface IEventNotificationHandler<in TEvent>
    where TEvent : notnull
{
    /// <summary>
    /// Handles the event notification asynchronously.
    /// </summary>
    /// <param name="notification">The event notification containing the event data.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(IEventNotification<TEvent> notification, CancellationToken ct = default);
}
