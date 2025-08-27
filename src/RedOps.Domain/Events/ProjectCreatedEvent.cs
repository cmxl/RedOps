using RedOps.Domain.Common.Events;

namespace RedOps.Domain.Events;

public class ProjectCreatedEvent : DomainEventBase
{
    public Guid ProjectId { get; }
    public string ProjectName { get; }

    public ProjectCreatedEvent(Guid projectId, string projectName)
    {
        ProjectId = projectId;
        ProjectName = projectName;
    }
}