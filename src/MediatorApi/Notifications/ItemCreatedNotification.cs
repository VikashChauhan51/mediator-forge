

using MediatR;

namespace MediatorApi.Notifications;

public class ItemCreatedNotification : INotification
{
    public Guid ItemId { get; set; }
}

public class ItemCreatedNotificationHandler(ILogger<ItemCreatedNotificationHandler> logger) : INotificationHandler<ItemCreatedNotification>
{
    public Task Handle(ItemCreatedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Notification:{notification.ItemId}");
        return Task.CompletedTask;
    }

     
}
