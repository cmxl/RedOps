namespace RedOps.Application.Common.Models;

public class AzureDevOpsProject
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string State { get; set; } = string.Empty;
    public DateTime LastUpdateTime { get; set; }
}

public class AzureWorkItem
{
    public int Id { get; set; }
    public string WorkItemType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string State { get; set; } = string.Empty;
    public string? Priority { get; set; }
    public string? AssignedTo { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime ChangedDate { get; set; }
    public Dictionary<string, object> Fields { get; set; } = new();
}

public class AzureComment
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

public class AzurePullRequest
{
    public int PullRequestId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
    public List<int> LinkedWorkItems { get; set; } = new();
}

public class AzureWorkItemCreateRequest
{
    public string WorkItemType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? State { get; set; }
    public string? Priority { get; set; }
    public string? AssignedTo { get; set; }
    public Dictionary<string, object> AdditionalFields { get; set; } = new();
}

public class AzureWorkItemUpdateRequest : AzureWorkItemCreateRequest
{
}