using RedOps.Domain.Common;
using RedOps.Domain.Enums;

namespace RedOps.Domain.Entities;

public class SyncOperation : Entity
{
    public Guid ProjectId { get; private set; }
    public OperationType OperationType { get; private set; }
    public SyncDirection SyncDirection { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public SyncStatus Status { get; private set; }
    public int ItemsProcessed { get; private set; }
    public int ErrorCount { get; private set; }
    public string? Details { get; private set; }
    public string? ErrorMessage { get; private set; }

    private SyncOperation(Guid id) : base(id) { }

    public static SyncOperation Create(Guid projectId, OperationType operationType, 
        SyncDirection syncDirection)
    {
        return new SyncOperation(Guid.NewGuid())
        {
            ProjectId = projectId,
            OperationType = operationType,
            SyncDirection = syncDirection,
            StartTime = DateTime.UtcNow,
            Status = SyncStatus.InProgress,
            ItemsProcessed = 0,
            ErrorCount = 0
        };
    }

    public void UpdateProgress(int itemsProcessed, int errorCount)
    {
        ItemsProcessed = itemsProcessed;
        ErrorCount = errorCount;
    }

    public void Complete(string? details = null)
    {
        Status = SyncStatus.Completed;
        EndTime = DateTime.UtcNow;
        Details = details;
    }

    public void Fail(string errorMessage, string? details = null)
    {
        Status = SyncStatus.Failed;
        EndTime = DateTime.UtcNow;
        ErrorMessage = errorMessage;
        Details = details;
    }
}