using Mediator;
using Microsoft.Extensions.Logging;
using RedOps.Domain.Events;

namespace RedOps.Application.Projects.EventHandlers;

public class ProjectMappingUpdatedEventHandler : INotificationHandler<ProjectMappingUpdatedEvent>
{
    private readonly ILogger<ProjectMappingUpdatedEventHandler> _logger;

    public ProjectMappingUpdatedEventHandler(ILogger<ProjectMappingUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask Handle(ProjectMappingUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Project mapping updated event handled: {ProjectId} - Redmine: {RedmineId}, AzureDevOps: {AzureDevOpsProject}", 
            notification.ProjectId, notification.RedmineId, notification.AzureDevOpsProject);

        // Additional side effects:
        // - Invalidate cached project mappings
        // - Trigger field mapping validation
        // - Update sync configuration
        // - Send mapping change notifications

        return ValueTask.CompletedTask;
    }
}