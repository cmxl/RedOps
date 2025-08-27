using RedOps.Domain.Common;
using RedOps.Domain.ValueObjects;

namespace RedOps.Domain.Entities;

public class Attachment : Entity
{
    public Guid WorkItemId { get; private set; }
    public RedmineId? RedmineId { get; private set; }
    public AzureDevOpsId? AzureDevOpsId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string? FileUrl { get; private set; }
    public DateTime CreatedUtc { get; private set; }
    public DateTime? LastSyncUtc { get; private set; }

    private Attachment(Guid id) : base(id) { }

    public static Attachment Create(Guid workItemId, string fileName, string contentType, 
        long fileSize, string? fileUrl)
    {
        return new Attachment(Guid.NewGuid())
        {
            WorkItemId = workItemId,
            FileName = fileName,
            ContentType = contentType,
            FileSize = fileSize,
            FileUrl = fileUrl,
            CreatedUtc = DateTime.UtcNow
        };
    }

    public void UpdateRedmineMapping(RedmineId redmineId, string? fileUrl)
    {
        RedmineId = redmineId;
        FileUrl = fileUrl ?? FileUrl;
        LastSyncUtc = DateTime.UtcNow;
    }

    public void UpdateAzureDevOpsMapping(AzureDevOpsId azureId, string? fileUrl)
    {
        AzureDevOpsId = azureId;
        FileUrl = fileUrl ?? FileUrl;
        LastSyncUtc = DateTime.UtcNow;
    }
}