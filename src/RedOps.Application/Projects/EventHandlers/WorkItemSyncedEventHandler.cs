using Mediator;
using Microsoft.Extensions.Logging;
using RedOps.Domain.Events;

namespace RedOps.Application.Projects.EventHandlers;

public class WorkItemSyncedEventHandler : INotificationHandler<WorkItemSyncedEvent>
{
    private readonly ILogger<WorkItemSyncedEventHandler> _logger;

    public WorkItemSyncedEventHandler(ILogger<WorkItemSyncedEventHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask Handle(WorkItemSyncedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("WorkItem synced event handled: {WorkItemId} in direction {SyncDirection}", 
            notification.WorkItemId, notification.SyncDirection);

        // Here we could trigger additional side effects like:
        // - Update sync statistics
        // - Send real-time notifications via SignalR
        // - Log detailed sync audit trail
        // - Trigger dependent sync operations

        return ValueTask.CompletedTask;
    }
}