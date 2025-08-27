using RedOps.Domain.Common;
using RedOps.Domain.Enums;

namespace RedOps.Domain.Entities;

public class SyncConflict : Entity
{
    public Guid WorkItemId { get; private set; }
    public ConflictType ConflictType { get; private set; }
    public string RedmineData { get; private set; } = string.Empty;
    public string AzureDevOpsData { get; private set; } = string.Empty;
    public string? Resolution { get; private set; }
    public string? ResolvedBy { get; private set; }
    public DateTime CreatedUtc { get; private set; }
    public DateTime? ResolvedUtc { get; private set; }
    public bool IsResolved { get; private set; }

    private SyncConflict(Guid id) : base(id) { }

    public static SyncConflict Create(Guid workItemId, ConflictType conflictType, 
        string redmineData, string azureDevOpsData)
    {
        return new SyncConflict(Guid.NewGuid())
        {
            WorkItemId = workItemId,
            ConflictType = conflictType,
            RedmineData = redmineData,
            AzureDevOpsData = azureDevOpsData,
            CreatedUtc = DateTime.UtcNow,
            IsResolved = false
        };
    }

    public void Resolve(string resolution, string resolvedBy)
    {
        Resolution = resolution;
        ResolvedBy = resolvedBy;
        ResolvedUtc = DateTime.UtcNow;
        IsResolved = true;
    }
}