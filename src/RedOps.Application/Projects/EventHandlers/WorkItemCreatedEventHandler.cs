using Mediator;
using Microsoft.Extensions.Logging;
using RedOps.Domain.Events;

namespace RedOps.Application.Projects.EventHandlers;

public class WorkItemCreatedEventHandler : INotificationHandler<WorkItemCreatedEvent>
{
    private readonly ILogger<WorkItemCreatedEventHandler> _logger;

    public WorkItemCreatedEventHandler(ILogger<WorkItemCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask Handle(WorkItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("WorkItem created event handled: {WorkItemId} in project {ProjectId} - {Title}", 
            notification.WorkItemId, notification.ProjectId, notification.Title);

        // Additional side effects:
        // - Update project statistics
        // - Send creation notifications
        // - Initialize work item tracking
        // - Trigger initial sync if configured

        return ValueTask.CompletedTask;
    }
}