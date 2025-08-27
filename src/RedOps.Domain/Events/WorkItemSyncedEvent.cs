using RedOps.Domain.Common.Events;
using RedOps.Domain.Enums;

namespace RedOps.Domain.Events;

public class WorkItemSyncedEvent : DomainEventBase
{
    public Guid WorkItemId { get; }
    public SyncDirection SyncDirection { get; }

    public WorkItemSyncedEvent(Guid workItemId, SyncDirection syncDirection)
    {
        WorkItemId = workItemId;
        SyncDirection = syncDirection;
    }
}