using Mediator;
using Microsoft.Extensions.Logging;
using RedOps.Domain.Events;

namespace RedOps.Application.Projects.EventHandlers;

public class ProjectDeactivatedEventHandler : INotificationHandler<ProjectDeactivatedEvent>
{
    private readonly ILogger<ProjectDeactivatedEventHandler> _logger;

    public ProjectDeactivatedEventHandler(ILogger<ProjectDeactivatedEventHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask Handle(ProjectDeactivatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Project deactivated event handled: {ProjectId}", 
            notification.ProjectId);

        // Additional side effects:
        // - Cancel any pending sync operations
        // - Archive project data
        // - Send deactivation notifications
        // - Update project statistics

        return ValueTask.CompletedTask;
    }
}