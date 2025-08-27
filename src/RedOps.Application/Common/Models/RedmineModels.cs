namespace RedOps.Application.Common.Models;

public class RedmineProject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}

public class RedmineWorkItem
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public RedmineStatus Status { get; set; } = new();
    public RedminePriority? Priority { get; set; }
    public RedmineUser? AssignedTo { get; set; }
    public RedmineUser Author { get; set; } = new();
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public List<RedmineCustomField> CustomFields { get; set; } = new();
}

public class RedmineStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class RedminePriority
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class RedmineUser
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
}

public class RedmineCustomField
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public object? Value { get; set; }
}

public class RedmineComment
{
    public int Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public RedmineUser User { get; set; } = new();
    public DateTime CreatedOn { get; set; }
}

public class RedmineWorkItemCreateRequest
{
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? StatusId { get; set; }
    public int? PriorityId { get; set; }
    public int? AssignedToId { get; set; }
    public List<RedmineCustomField> CustomFields { get; set; } = new();
}

public class RedmineWorkItemUpdateRequest : RedmineWorkItemCreateRequest
{
    public string? Notes { get; set; }
}