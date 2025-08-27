using RedOps.Domain.Common.Events;

namespace RedOps.Domain.Events;

public class WorkItemCreatedEvent : DomainEventBase
{
    public Guid WorkItemId { get; }
    public Guid ProjectId { get; }
    public string Title { get; }

    public WorkItemCreatedEvent(Guid workItemId, Guid projectId, string title)
    {
        WorkItemId = workItemId;
        ProjectId = projectId;
        Title = title;
    }
}