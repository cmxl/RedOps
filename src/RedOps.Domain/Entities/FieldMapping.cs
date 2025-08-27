using RedOps.Domain.Common;

namespace RedOps.Domain.Entities;

public class FieldMapping : Entity
{
    public Guid ProjectId { get; private set; }
    public string RedmineField { get; private set; } = string.Empty;
    public string AzureDevOpsField { get; private set; } = string.Empty;
    public string? MappingRule { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedUtc { get; private set; }
    public DateTime ModifiedUtc { get; private set; }

    private FieldMapping(Guid id) : base(id) { }

    public static FieldMapping Create(Guid projectId, string redmineField, 
        string azureDevOpsField, string? mappingRule = null)
    {
        return new FieldMapping(Guid.NewGuid())
        {
            ProjectId = projectId,
            RedmineField = redmineField,
            AzureDevOpsField = azureDevOpsField,
            MappingRule = mappingRule,
            IsActive = true,
            CreatedUtc = DateTime.UtcNow,
            ModifiedUtc = DateTime.UtcNow
        };
    }

    public void UpdateMappingRule(string? mappingRule)
    {
        MappingRule = mappingRule;
        ModifiedUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        ModifiedUtc = DateTime.UtcNow;
    }
}