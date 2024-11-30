

using MediatorForge.CQRS.Notifications;
using MediatR;

namespace MediatorApi.Notifications;

public class ItemCreatedNotification 
{
    public Guid ItemId { get; set; }
}

public class ItemCreatedNotificationHandler(ILogger<ItemCreatedNotificationHandler> logger) : INotificationHandler<EventNotification<ItemCreatedNotification>>
{
    public Task Handle(EventNotification<ItemCreatedNotification> notification, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Notification:{notification.Event}");
        return Task.CompletedTask;
    }

     
}
