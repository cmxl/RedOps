using RedOps.Domain.Common.Events;

namespace RedOps.Domain.Events;

public class ProjectDeactivatedEvent : DomainEventBase
{
    public Guid ProjectId { get; }

    public ProjectDeactivatedEvent(Guid projectId)
    {
        ProjectId = projectId;
    }
}