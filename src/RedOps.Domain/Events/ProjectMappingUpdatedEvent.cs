using RedOps.Domain.Common.Events;
using RedOps.Domain.ValueObjects;

namespace RedOps.Domain.Events;

public class ProjectMappingUpdatedEvent : DomainEventBase
{
    public Guid ProjectId { get; }
    public RedmineId? RedmineId { get; }
    public string? AzureDevOpsProject { get; }

    public ProjectMappingUpdatedEvent(Guid projectId, RedmineId? redmineId, string? azureDevOpsProject)
    {
        ProjectId = projectId;
        RedmineId = redmineId;
        AzureDevOpsProject = azureDevOpsProject;
    }
}