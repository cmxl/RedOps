using RedOps.Domain.Enums;

namespace RedOps.Application.Common.DTOs;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? RedmineId { get; set; }
    public string? AzureDevOpsProject { get; set; }
    public SyncDirection SyncDirection { get; set; }
    public DateTime? LastSyncUtc { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime ModifiedUtc { get; set; }
    public int WorkItemCount { get; set; }
    public int UnresolvedConflictsCount { get; set; }
}

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? RedmineId { get; set; }
    public string? AzureDevOpsProject { get; set; }
    public SyncDirection SyncDirection { get; set; }
}

public class UpdateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? RedmineId { get; set; }
    public string? AzureDevOpsProject { get; set; }
    public SyncDirection SyncDirection { get; set; }
}