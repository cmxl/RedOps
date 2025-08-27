using RedOps.Domain.Common;
using RedOps.Domain.Enums;
using RedOps.Domain.Events;
using RedOps.Domain.ValueObjects;

namespace RedOps.Domain.Entities;

public class Project : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public RedmineId? RedmineId { get; private set; }
    public string? AzureDevOpsProject { get; private set; }
    public SyncDirection SyncDirection { get; private set; }
    public DateTime? LastSyncUtc { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedUtc { get; private set; }
    public DateTime ModifiedUtc { get; private set; }

    private readonly List<WorkItem> _workItems = new();
    private readonly List<FieldMapping> _fieldMappings = new();

    public IReadOnlyCollection<WorkItem> WorkItems => _workItems.AsReadOnly();
    public IReadOnlyCollection<FieldMapping> FieldMappings => _fieldMappings.AsReadOnly();

    private Project(Guid id) : base(id) { }

    public static Project Create(string name, string? description, SyncDirection syncDirection)
    {
        var project = new Project(Guid.NewGuid())
        {
            Name = name,
            Description = description,
            SyncDirection = syncDirection,
            IsActive = true,
            CreatedUtc = DateTime.UtcNow,
            ModifiedUtc = DateTime.UtcNow
        };

        project.AddDomainEvent(new ProjectCreatedEvent(project.Id, project.Name));
        return project;
    }

    public void UpdateRedmineMapping(RedmineId redmineId)
    {
        RedmineId = redmineId;
        ModifiedUtc = DateTime.UtcNow;
        AddDomainEvent(new ProjectMappingUpdatedEvent(Id, redmineId, null));
    }

    public void UpdateAzureDevOpsMapping(string azureDevOpsProject)
    {
        AzureDevOpsProject = azureDevOpsProject;
        ModifiedUtc = DateTime.UtcNow;
        AddDomainEvent(new ProjectMappingUpdatedEvent(Id, null, azureDevOpsProject));
    }

    public void UpdateSyncTimestamp()
    {
        LastSyncUtc = DateTime.UtcNow;
        ModifiedUtc = DateTime.UtcNow;
    }

    public void AddFieldMapping(FieldMapping fieldMapping)
    {
        _fieldMappings.Add(fieldMapping);
        ModifiedUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        ModifiedUtc = DateTime.UtcNow;
        AddDomainEvent(new ProjectDeactivatedEvent(Id));
    }
}