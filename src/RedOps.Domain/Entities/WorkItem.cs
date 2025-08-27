using RedOps.Domain.Common;
using RedOps.Domain.Enums;
using RedOps.Domain.Events;
using RedOps.Domain.ValueObjects;

namespace RedOps.Domain.Entities;

public class WorkItem : AggregateRoot
{
    public Guid ProjectId { get; private set; }
    public RedmineId? RedmineId { get; private set; }
    public AzureDevOpsId? AzureDevOpsId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public string? Priority { get; private set; }
    public string? AssigneeEmail { get; private set; }
    public DateTime CreatedUtc { get; private set; }
    public DateTime ModifiedUtc { get; private set; }
    public DateTime? LastSyncUtc { get; private set; }
    public string? RedmineData { get; private set; }
    public string? AzureDevOpsData { get; private set; }

    private readonly List<Comment> _comments = new();
    private readonly List<Attachment> _attachments = new();

    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();
    public IReadOnlyCollection<Attachment> Attachments => _attachments.AsReadOnly();

    private WorkItem(Guid id) : base(id) { }

    public static WorkItem Create(Guid projectId, string title, string? description, string status)
    {
        var workItem = new WorkItem(Guid.NewGuid())
        {
            ProjectId = projectId,
            Title = title,
            Description = description,
            Status = status,
            CreatedUtc = DateTime.UtcNow,
            ModifiedUtc = DateTime.UtcNow
        };

        workItem.AddDomainEvent(new WorkItemCreatedEvent(workItem.Id, workItem.ProjectId, workItem.Title));
        return workItem;
    }

    public void UpdateFromRedmine(RedmineId redmineId, string title, string? description, 
        string status, string? priority, string? assigneeEmail, string redmineData)
    {
        RedmineId = redmineId;
        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
        AssigneeEmail = assigneeEmail;
        RedmineData = redmineData;
        ModifiedUtc = DateTime.UtcNow;
        LastSyncUtc = DateTime.UtcNow;

        AddDomainEvent(new WorkItemSyncedEvent(Id, SyncDirection.FromRedmine));
    }

    public void UpdateFromAzureDevOps(AzureDevOpsId azureId, string title, string? description,
        string status, string? priority, string? assigneeEmail, string azureData)
    {
        AzureDevOpsId = azureId;
        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
        AssigneeEmail = assigneeEmail;
        AzureDevOpsData = azureData;
        ModifiedUtc = DateTime.UtcNow;
        LastSyncUtc = DateTime.UtcNow;

        AddDomainEvent(new WorkItemSyncedEvent(Id, SyncDirection.ToRedmine));
    }

    public void AddComment(Comment comment)
    {
        _comments.Add(comment);
        ModifiedUtc = DateTime.UtcNow;
    }

    public void AddAttachment(Attachment attachment)
    {
        _attachments.Add(attachment);
        ModifiedUtc = DateTime.UtcNow;
    }
}