using RedOps.Domain.Common;
using RedOps.Domain.ValueObjects;

namespace RedOps.Domain.Entities;

public class Comment : Entity
{
    public Guid WorkItemId { get; private set; }
    public RedmineId? RedmineId { get; private set; }
    public AzureDevOpsId? AzureDevOpsId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public string AuthorEmail { get; private set; } = string.Empty;
    public DateTime CreatedUtc { get; private set; }
    public DateTime? LastSyncUtc { get; private set; }

    private Comment(Guid id) : base(id) { }

    public static Comment Create(Guid workItemId, string content, string authorEmail)
    {
        return new Comment(Guid.NewGuid())
        {
            WorkItemId = workItemId,
            Content = content,
            AuthorEmail = authorEmail,
            CreatedUtc = DateTime.UtcNow
        };
    }

    public void UpdateRedmineMapping(RedmineId redmineId)
    {
        RedmineId = redmineId;
        LastSyncUtc = DateTime.UtcNow;
    }

    public void UpdateAzureDevOpsMapping(AzureDevOpsId azureId)
    {
        AzureDevOpsId = azureId;
        LastSyncUtc = DateTime.UtcNow;
    }
}