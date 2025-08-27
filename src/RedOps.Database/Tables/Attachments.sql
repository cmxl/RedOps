CREATE TABLE [dbo].[Attachments] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [WorkItemId] UNIQUEIDENTIFIER NOT NULL,
    [FileName] NVARCHAR(255) NOT NULL,
    [FileSize] BIGINT NOT NULL,
    [ContentType] NVARCHAR(100) NULL,
    [FileHash] NVARCHAR(64) NULL, -- SHA-256 hash for deduplication
    [StoragePath] NVARCHAR(500) NULL, -- Path in blob storage or file system
    [RedmineId] INT NULL,
    [AzureDevOpsId] UNIQUEIDENTIFIER NULL,
    [RedmineDownloadUrl] NVARCHAR(500) NULL,
    [AzureDevOpsUrl] NVARCHAR(500) NULL,
    [IsDownloaded] BIT NOT NULL DEFAULT 0,
    [LastSyncUtc] DATETIME2(7) NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] NVARCHAR(255) NULL,
    
    CONSTRAINT [PK_Attachments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Attachments_WorkItemId] FOREIGN KEY ([WorkItemId]) REFERENCES [dbo].[WorkItems] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [UQ_Attachments_WorkItemId_RedmineId] UNIQUE ([WorkItemId], [RedmineId]),
    CONSTRAINT [UQ_Attachments_WorkItemId_AzureDevOpsId] UNIQUE ([WorkItemId], [AzureDevOpsId]),
    CONSTRAINT [CK_Attachments_FileSize] CHECK ([FileSize] > 0)
);