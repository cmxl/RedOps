using RedOps.Domain.Enums;

namespace RedOps.Application.Common.DTOs;

public class ConflictDto
{
    public Guid Id { get; set; }
    public Guid WorkItemId { get; set; }
    public string WorkItemTitle { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public ConflictType ConflictType { get; set; }
    public string RedmineData { get; set; } = string.Empty;
    public string AzureDevOpsData { get; set; } = string.Empty;
    public string? Resolution { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? ResolvedUtc { get; set; }
    public bool IsResolved { get; set; }
    public bool CanAutoResolve { get; set; }
    public string? SuggestedResolution { get; set; }
}

public class ResolveConflictDto
{
    public Guid ConflictId { get; set; }
    public string Resolution { get; set; } = string.Empty;
    public string ResolvedBy { get; set; } = string.Empty;
}