using RedOps.Domain.Entities;
using RedOps.Domain.Enums;

namespace RedOps.Domain.Services;

public interface IConflictResolutionService
{
    Task<ConflictResolution> DetectConflictAsync(WorkItem workItem, string redmineData, string azureDevOpsData);
    Task<SyncConflict> CreateConflictAsync(Guid workItemId, ConflictType conflictType, string redmineData, string azureDevOpsData);
    Task<bool> CanAutoResolveAsync(SyncConflict conflict);
    Task<string> GenerateResolutionAsync(SyncConflict conflict, string strategy);
}

public class ConflictResolution
{
    public bool HasConflict { get; set; }
    public ConflictType ConflictType { get; set; }
    public string? Description { get; set; }
    public bool CanAutoResolve { get; set; }
    public string? SuggestedResolution { get; set; }
}