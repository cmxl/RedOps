namespace RedOps.Application.Common.DTOs;

public class WorkItemDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public int? RedmineId { get; set; }
    public int? AzureDevOpsId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Priority { get; set; }
    public string? AssigneeEmail { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime ModifiedUtc { get; set; }
    public DateTime? LastSyncUtc { get; set; }
    public int CommentCount { get; set; }
    public int AttachmentCount { get; set; }
    public bool HasConflicts { get; set; }
}

public class CreateWorkItemDto
{
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Priority { get; set; }
    public string? AssigneeEmail { get; set; }
}

public class UpdateWorkItemDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Priority { get; set; }
    public string? AssigneeEmail { get; set; }
}