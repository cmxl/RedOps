using RedOps.Domain.Enums;

namespace RedOps.Application.Common.DTOs;

public class SyncOperationDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public OperationType OperationType { get; set; }
    public SyncDirection SyncDirection { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public SyncStatus Status { get; set; }
    public int ItemsProcessed { get; set; }
    public int ErrorCount { get; set; }
    public string? Details { get; set; }
    public string? ErrorMessage { get; set; }
    public TimeSpan? Duration => EndTime?.Subtract(StartTime);
}

public class SyncStatusDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public bool IsSyncInProgress { get; set; }
    public DateTime? LastSyncUtc { get; set; }
    public SyncOperationDto? CurrentOperation { get; set; }
    public List<SyncOperationDto> RecentOperations { get; set; } = new();
    public int PendingItemsCount { get; set; }
    public int ConflictsCount { get; set; }
}