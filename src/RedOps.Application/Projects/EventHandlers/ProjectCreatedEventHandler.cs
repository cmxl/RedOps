using Mediator;
using Microsoft.Extensions.Logging;
using RedOps.Domain.Events;

namespace RedOps.Application.Projects.EventHandlers;

public class ProjectCreatedEventHandler : INotificationHandler<ProjectCreatedEvent>
{
    private readonly ILogger<ProjectCreatedEventHandler> _logger;

    public ProjectCreatedEventHandler(ILogger<ProjectCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask Handle(ProjectCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Project created event handled: {ProjectId} - {ProjectName}", 
            notification.ProjectId, notification.ProjectName);

        // Here we could trigger additional side effects like:
        // - Send notifications
        // - Initialize default field mappings
        // - Log audit events
        // - Trigger initial sync setup

        return ValueTask.CompletedTask;
    }
}